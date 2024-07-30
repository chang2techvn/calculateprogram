using System.Collections;
using System.Windows.Forms;

namespace CalculateProgram
{
    public class ListViewItemComparer : IComparer
    {
        private int col;
        private bool ascending;

        public ListViewItemComparer(int column, bool ascending = true)
        {
            col = column;
            this.ascending = ascending;
        }

        public int Compare(object x, object y)
        {
            string textX = ((ListViewItem)x).SubItems[col].Text;
            string textY = ((ListViewItem)y).SubItems[col].Text;

            // Convert text to numeric values for sorting, if necessary
            double valX, valY;
            if (double.TryParse(textX, out valX) && double.TryParse(textY, out valY))
            {
                return ascending ? valX.CompareTo(valY) : valY.CompareTo(valX);
            }
            else
            {
                return ascending ? string.Compare(textX, textY) : string.Compare(textY, textX);
            }
        }

    }
}
