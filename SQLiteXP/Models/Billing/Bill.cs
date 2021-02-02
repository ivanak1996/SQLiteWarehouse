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
        public int year { get; private set; }
        public string docType { get; private set; }
        public int ordinaryNumber { get; private set; }

        // payment details - sta je uplaceno preko cega
        public float cek { get; set; }
        public float gotovina { get; set; }
        public float virman { get; set; }
        public float kartica { get; set; }
        public float uplaceno { get; set; }
        public float povracaj { get; set; }
        public float iznos { get; set; }
        public float popust { get; set; }

        private bool itemsLoaded = false;

        public string GetBillNumber()
        {
            return $"{year}-{docType}-{ordinaryNumber}";
        }

        public Bill() { }

        public Bill(int year, string docType, int ordinaryNumber)
        {
            this.year = year;
            this.docType = docType;
            this.ordinaryNumber = ordinaryNumber;
        }

        //todo consider storing total price as a field
        public float TotalPrice()
        {
            return Items.Sum(i => i.Quantity * i.productCena - i.productPopust);
        }

        public float RacunBezPopusta()
        {
            return Items.Sum(i => i.Quantity * i.productCena);
        }

        public float Popust()
        {
            return Items.Sum(i => i.productPopust);
        }

        public void LoadItems()
        {
            if (!itemsLoaded)
            {
                Items = SQLiteDataAccess.GetAllBillItemsForBill(Id);
                itemsLoaded = true;
            }
        }

        public void AddItem(BillItem item)
        {
            BillItem existingItem = Items.FirstOrDefault(i => i.productIdent == item.productIdent);
            if (existingItem == null && item.Quantity > 0)
            {
                Items.Add(item);
                SQLiteDataAccess.InsertBillItem(item);
            }
            else
            {                
                if (existingItem.Quantity + item.Quantity > 0)
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

        public void UpdateItem(BillItem item)
        {
            if (item.Quantity > 0)
            {
                SQLiteDataAccess.UpdateItem(item);
            }
            else
            {
                RemoveItem(item);
            }
        }

        public void RemoveItem(BillItem item)
        {
            Items.Remove(item);
            SQLiteDataAccess.DeleteItem(item);
        }

        public void Save()
        {
            SQLiteDataAccess.UpdateBill(this);
        }
    }
}
