using SQLiteXP.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteXP.Models.Billing
{
    public class Bill
    {
        public const string STATUS_OPEN = "Open";
        public const string STATUS_COMPLETE = "Complete";

        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string Status { get; set; }
        public int Id { get; set; }
        public IList<BillItem> Items { get; set; } = new List<BillItem>();

        //todo consider storing total price as a field
        public float TotalPrice()
        {
            return Items.Sum(i => i.Quantity * i.productCena);
        }

        public void AddItem(BillItem item)
        {
            BillItem existingItem = Items.FirstOrDefault(i => i.productIdent == item.productIdent);
            if (existingItem == null && item.Quantity != 0)
            {
                Items.Add(item);
                SQLiteDataAccess.InsertBillItem(item);
            }
            else
            {                
                if (existingItem.Quantity + item.Quantity != 0)
                {
                    existingItem.Quantity += item.Quantity;
                    SQLiteDataAccess.UpdateItem(existingItem);
                }
                else
                {
                    RemoveItem(existingItem);
                }
            }
        }

        public void RemoveItem(BillItem item)
        {
            Items.Remove(item);
            SQLiteDataAccess.DeleteItem(item);
        }
    }
}
