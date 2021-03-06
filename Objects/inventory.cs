using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Inventory.Objects
{
  public class InventoryItem
  {
    private int _id;
    private string _name;
    private string _description;

    public InventoryItem(string name, string description, int Id = 0)
    {
      _id = Id;
      SetName(name);
      SetDescription(description);
    }

    public override bool Equals(System.Object otherInventoryItem)
    {
      if(!(otherInventoryItem is InventoryItem)) return false;
      else
      {
        InventoryItem newInventoryItem = (InventoryItem) otherInventoryItem;
        bool idEquality = (this.GetId() == newInventoryItem.GetId());
        bool nameEquality = (this.GetName() == newInventoryItem.GetName());
        return (nameEquality);
      }
    }

    public int GetId()
    {
      return _id;
    }

    public void SetName(string name)
    {
      _name = name;
    }

    public string GetName()
    {
      return _name;
    }

    public void SetDescription(string description)
    {
      _description = description;
    }

    public string GetDescription()
    {
      return _description;
    }

    public static List<InventoryItem> GetAll()
    {
      List<InventoryItem> allInventoryItems = new List<InventoryItem>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("Select * FROM InventoryItems;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int inventoryItemId = rdr.GetInt32(0);
        string inventoryItemName = rdr.GetString(1);
        string inventoryItemDescription = rdr.GetString(2);
        InventoryItem newInventoryItem = new InventoryItem(inventoryItemName, inventoryItemDescription, inventoryItemId);
        allInventoryItems.Add(newInventoryItem);
      }

      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();
      return allInventoryItems;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO InventoryItems (name, description) OUTPUT INSERTED.id VALUES (@ItemName, @ItemDescription);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@ItemName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);
      SqlParameter descriptionParameter = new SqlParameter();
      descriptionParameter.ParameterName = "@ItemDescription";
      descriptionParameter.Value = this.GetDescription();
      cmd.Parameters.Add(descriptionParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read()) this._id = rdr.GetInt32(0);
      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM InventoryItems;", conn);
      cmd.ExecuteNonQuery();
    }

    public static void DeleteOne(string name)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      // string commandWithVariableName = string.Format("DELETE FROM InventoryItems WHERE name= {0};", name);
      SqlCommand cmd = new SqlCommand("DELETE FROM inventoryItems WHERE name= @ItemName;", conn);
      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@ItemName";
      nameParameter.Value = name;
      cmd.Parameters.Add(nameParameter);
      cmd.ExecuteNonQuery();
      if (conn != null) conn.Close();
    }

    public static InventoryItem Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM inventoryItems WHERE id= @InventoryItemID;", conn);
      SqlParameter inventoryItemIdParameter = new SqlParameter();
      inventoryItemIdParameter.ParameterName = "@InventoryItemId";
      inventoryItemIdParameter.Value = id.ToString();
      cmd.Parameters.Add(inventoryItemIdParameter);
      rdr = cmd.ExecuteReader();

      int foundInventoryItemId = 0;
      string foundInventoryItemName = null;
      string foundInventoryItemDescription = null;
      while(rdr.Read())
      {
        foundInventoryItemId = rdr.GetInt32(0);
        foundInventoryItemName = rdr.GetString(1);
      }
      InventoryItem foundInventoryItem = new InventoryItem(foundInventoryItemName, foundInventoryItemDescription, foundInventoryItemId);
      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();
      return foundInventoryItem;
    }

    public static int Search(string searchName)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM inventoryItems WHERE name= @ItemName;", conn);
      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@ItemName";
      nameParameter.Value = searchName;
      cmd.Parameters.Add(nameParameter);
      rdr = cmd.ExecuteReader();

      int foundInventoryItemId = -1;
      while(rdr.Read()) foundInventoryItemId = rdr.GetInt32(0);
      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();
      return foundInventoryItemId;
    }
  }
}
