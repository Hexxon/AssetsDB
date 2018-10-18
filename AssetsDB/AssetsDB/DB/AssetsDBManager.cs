using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;


namespace AssetsDB.DB
{
   public class AssetsDBManager
   {
      #region READ - DB

      // return all entries
      public static IList<Asset> All()
      {
         return DoCommands(col => col.FindAll().ToList());
      }

      /// <summary>
      ///  Find entries by asset short name, this is case sensitive.
      /// </summary>
      /// <param name="strAssetName">Case sensitive asset name</param>
      /// <returns></returns>
      public static IList<Asset> GetByName(string strAssetName)
      {
         return DoCommands(col => {
            var result = col.Find(Query.EQ("Name", strAssetName));
            return result.ToList();
         });
      }

      // find entries by asset db id
      public static Asset GetById(int assetId)
      {
         return DoCommands(col => {
            var result = col.FindById(assetId);
            return result;
         });
      }

      #endregion

      #region WRITE - DB

      public static int DeleteAssets(params Asset[] assetsToDelete)
      {
         return DeleteAssets(assetsToDelete.Select(x => x.Id).ToArray());
      }

      public static int DeleteAssets( params int[] assetsIdToDelete )
      {
         return DoCommands(col => col.Delete(x => assetsIdToDelete.Contains(x.Id)));
      }

      public static void InsertAssets(params Asset[] assetsToAdd)
      {
         DoCommands(col => { col.Insert(assetsToAdd); });
      }

      public static void UpdateAssets(params Asset[] assetsToUpdate)
      {
         DoCommands(col => { col.Update(assetsToUpdate); });
      }

      #endregion

      #region BASIC - DB - ACCESS

      public static string DBConnectionString { get; set; }

      private const string STR_COLLECTION_NAME = "assets";

      private static void DoCommands(Action<LiteCollection<Asset>> cmdsToRun)
      {
         if (cmdsToRun == null) {
            return;
         }

         // Open database or create if doesn't exist
         using (var db = new LiteDatabase(DBConnectionString)) {
            // Get a collection (or create, if doesn't exist)
            LiteCollection<Asset> col = db.GetCollection<Asset>(STR_COLLECTION_NAME);
            cmdsToRun.Invoke(col);
         }
      }

      private static T DoCommands<T>(Func<LiteCollection<Asset>, T> cmdsToRun)
      {
         if (cmdsToRun == null) {
            return default(T);
         }

         // Open database or create if doesn't exist
         using (var db = new LiteDatabase(DBConnectionString)) {
            // Get a collection (or create, if doesn't exist)
            LiteCollection<Asset> col = db.GetCollection<Asset>(STR_COLLECTION_NAME);

            return cmdsToRun.Invoke(col);
         }
      }

      #endregion
   }
}
