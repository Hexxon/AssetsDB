using System;
using System.Linq;
using System.Net;

using AssetsDB.DB;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;


namespace AssetsDB.Controllers
{
   [Produces("application/json")]
   [Route("api/v1")]
   [ApiController]
   public class AssetsDBController : ControllerBase
   {
      #region READ

      // GET api/v1
      /// <summary>
      /// Get all entries.
      /// </summary>
      /// <returns></returns>
      [HttpGet]
      public IActionResult GetAll()
      {
         var result = AssetsDBManager.All();
         return Ok(result);

      }

      // GET api/v1/search/{searchString}
      /// <summary>
      /// Get all assets that contains the given search string in there name or long name field
      /// </summary>
      /// <param name="searchString"></param>
      /// <returns></returns>
      [HttpGet("search/{searchString}")]
      public IActionResult Search(string searchString)
      {
         if (string.IsNullOrWhiteSpace(searchString)) {
            return Ok(null);
         }

         var allAssets = AssetsDBManager.All();
         var searchResult = allAssets.Where(x =>
            x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
            x.LongName.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));

         return Ok(searchResult);
      }

      // GET api/v1/<assetname>
      /// <summary>
      /// Get entry by asset name.
      /// </summary>
      /// <param name="assetName"></param>
      /// <returns></returns>
      [HttpGet("{assetName}", Name = "GetByName")]
      public IActionResult GetByName(string assetName)
      {
         var result = AssetsDBManager.GetByName(assetName);
         return Ok(result);
      }

      // GET api/v1/<assetId>
      /// <summary>
      /// Get asset by record id.
      /// </summary>
      /// <param name="assetId"></param>
      /// <returns></returns>
      [HttpGet("{assetId:int}", Name = "GetById")]
      public IActionResult GetById(int assetId)
      {
         var result = AssetsDBManager.GetById(assetId);
         if (result == null) {
            return NotFound();
         }

         return Ok(result);
      }

      #endregion

      #region Create

      // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.1
      // POST api/v1
      /// <summary>
      /// Add new asset.
      /// </summary>
      /// <param name="value"></param>
      /// <returns>Returns the newly added asset</returns>
      [HttpPost]
      [SwaggerResponse(HttpStatusCode.Created, typeof(Asset), Description = "Returns the newly added asset.")]
      [SwaggerResponse(HttpStatusCode.Conflict, typeof(Asset), Description = "If there is already an asset with the same name.")]
      [SwaggerResponse(HttpStatusCode.UnprocessableEntity, typeof(string), Description = "If the asset have empty required fields.")]
      [SwaggerResponse(HttpStatusCode.BadRequest, null, Description = "If the asset is null.", IsNullable = true)]
      public IActionResult AddAsset(Asset value)
      {
         if (value == null) {
            return BadRequest();
         }

         // required field
         if (string.IsNullOrEmpty(value.Name)) {
            return UnprocessableEntity("Name field cannot be empty.");
         }

         // required field
         if (string.IsNullOrEmpty(value.LongName)) {
            return UnprocessableEntity("LongName field cannot be empty.");
         }

         // check if we already have this asset
         var existingAssets = AssetsDBManager.GetByName(value.Name);
         if (existingAssets.Count > 0) {
            return Conflict(existingAssets[0]);
         }

         // create new entry
         value.Id = 0; // it will be generated..
         AssetsDBManager.InsertAssets(value);

         return CreatedAtRoute("GetById", new { assetId = value.Id }, value);
      }

      #endregion

      #region Update

      // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.1
      // PUT api/v1/
      /// <summary>
      /// Update existing asset. Field Name will not be updated.
      /// </summary>
      /// <returns>Asset updated.</returns>
      [HttpPut]
      [SwaggerResponse( HttpStatusCode.BadRequest, null, Description = "If the asset is null.", IsNullable = true )]
      [SwaggerResponse( HttpStatusCode.NotFound, null, Description = "If there is no asset with the given id.", IsNullable = true )]
      [SwaggerResponse( HttpStatusCode.UnprocessableEntity, typeof( string ), Description = "If the asset have empty required fields." )]
      [SwaggerResponse( HttpStatusCode.OK, null, Description = "Asset was updated.", IsNullable = true )]
      public IActionResult UpdateAsset( Asset value )
      {
         if ( value == null ) {
            return BadRequest();
         }

         var existingAsset = AssetsDBManager.GetById( value.Id );
         if ( existingAsset == null ) {
            return NotFound();
         }

         // required field
         if ( string.IsNullOrEmpty( value.LongName ) ) {
            return UnprocessableEntity( "LongName field cannot be empty." );
         }

         existingAsset.LongName = value.LongName;
         existingAsset.MarketsInfo = value.MarketsInfo;
         existingAsset.Urls = value.Urls;
         existingAsset.Note = value.Note;

         // update...
         AssetsDBManager.UpdateAssets( existingAsset );

         return Ok();
      }

      #endregion

      #region Delete
      // DELETE api/v1/{assetId}
      /// <summary>
      /// Delete asset by id.
      /// </summary>
      /// <param name="assetId">Asset id</param>
      /// <returns>Asset removed.</returns>
      [HttpDelete("{assetId:int}")]
      [SwaggerResponse(HttpStatusCode.OK, null)]
      public IActionResult DeleteById(int assetId)
      {
         //  delete asset from DB.
         AssetsDBManager.DeleteAssets(assetId);
         return Ok();
      }

      #endregion
   }
}
