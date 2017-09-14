using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Lib;
using Clipboard = System.Windows.Clipboard;
using ContextMenu = System.Windows.Controls.ContextMenu;
using DataGrid = System.Windows.Controls.DataGrid;
using MenuItem = System.Windows.Controls.MenuItem;

namespace Lib
{
    public static class Utils
    {

        public class RowComparer: IComparer
        {
            private int col;
            public RowComparer(int col)
            {
                this.col = col;
            }
            int IComparer.Compare(object a, object b)
            {
                return CompareRows(a, b, col);
            }
        }
        public class InvRowComparer : IComparer
        {
            private int col;
            public InvRowComparer(int col)
            {
                this.col = col;
            }
            int IComparer.Compare(object a, object b)
            {
                return -CompareRows(a, b, col);
            }
        }
        public static int CompareRows(object a, object b, int col)
        {
            object[] a1 = (object[]) a;
            if (a1 == null) return -1;
            object[] b1 = (object[]) b;
            return (a1[col] as IComparable).CompareTo(b1[col]);  
        }

        public class RowComparer<T> : IComparer
        {
            //private const STR_M_QER = "";
            private int col;
            public RowComparer(int col)
            {
                this.col = col;
            }
            int IComparer.Compare(object a, object b)
            {
                T[] a1 = (T[])a;
                if (a1 == null) return -1;
                T[] b1 = (T[])b;
                return (a1[col] as IComparable).CompareTo(b1[col]);
            }
        }
        public class RowTComparer<T>: IComparer
        {
            //private const STR_M_QER = "";
            public int col;
            public T value;
            public RowTComparer(int col, T value)
            {
                this.value = value;
                this.col = col;
            }
            int IComparer.Compare(object source, object b)
            {
                T[] sourceRow = source as T[];
                if (sourceRow == null) return -1;
                return (sourceRow[col] as IComparable).CompareTo(value);
            }
        }

        public static bool EqualByElements<T>(this T[] items, T[] second)
        {
            if (second == null) return false;
            if (items.Length != second.Length) return false;
            for (int i = 0; i < items.Length; i++)
                if (!items[i].Equals(second[i])) return false;
            return true;
        }
        public static IEnumerable<List<T>> ToChunks<T>(this IEnumerable<T> items, int chunkSize) 
        {
            List<T> chunk = new List<T>(chunkSize);
            foreach (var item in items) 
            {
                chunk.Add(item);
                if (chunk.Count == chunkSize) 
                {
                    yield return chunk;
                    chunk = new List<T>(chunkSize);
                }
            }
            if (chunk.Any()) yield return chunk;
        }
        public static string GetCurCell(DataGrid dg)
        {
            if (dg.CurrentCell.Column == null) return "";
            int selCol = dg.CurrentCell.Column.DisplayIndex;
            var selCell = dg.SelectedCells[selCol];
            var cellContent = selCell.Column.GetCellContent(selCell.Item);
            if (cellContent is TextBlock) return ((TextBlock)cellContent).Text;
            return "";
        }
        public static IEnumerable<string> ClipBoardStrs()
        {
            if (!Clipboard.ContainsText()) return new string[0];
            string txt = Clipboard.GetText();
            string[] cells = txt.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return cells;
        }

        public static T[] SelItemsFromDg<T>(DataGrid dg)
        {
            List<T> items = new List<T>();
            if (dg.SelectionUnit == DataGridSelectionUnit.Cell)
                items.AddRange(dg.SelectedCells.Select(c => c.Item).OfType<T>());
            else
                items.AddRange(dg.SelectedItems.OfType<T>());
            if (items.Count > 0) return items.ToArray();
            T[] pars = dg.Items.OfType<T>().Select(s => s).ToArray();
            return pars;
        }
        public static DataGrid GridByMenuItem(object menuItem)
        {
            var mnuItem = menuItem as MenuItem;
            ContextMenu mnu = VisualTreeHelpers.FindAncestor<ContextMenu>(mnuItem);
            var dg = mnu.PlacementTarget as DataGrid;
            return dg;
        }
    }




}
