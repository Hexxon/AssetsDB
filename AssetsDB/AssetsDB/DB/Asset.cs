using System.Collections.Generic;

using LiteDB;


namespace AssetsDB.DB
{
   public class AssetOnXchgInfo
   {
      // xchg name
      public string XchgName { get; set; } = string.Empty;

      // asset short name used on this xchg
      public string AssetName { get; set; } = string.Empty;

      public override string ToString()
      {
         return $"{XchgName}::{AssetName}";
      }
   }


   public class Asset
   {
      [BsonId(true)]
      public int Id { get; set; }

      // asset short name
      public string Name { get; set; } = string.Empty;

      // asset long name
      public string LongName { get; set; } = string.Empty;

      // list of xchg where this assets is available 
      public List<AssetOnXchgInfo> MarketsInfo { get; set; } = new List<AssetOnXchgInfo>();

      // asset's urls, if it has one..
      public List<string> Urls { get; set; } = new List<string>();

      // any note, for example from where the information was taken...
      public string Note { get; set; } = string.Empty;

      public override string ToString()
      {
         return $"Name:{Name} LongName:{LongName} Markets:{string.Join(":::", MarketsInfo)} Urls:{string.Join("::", Urls)} Note:{Note}";
      }
   }
}
