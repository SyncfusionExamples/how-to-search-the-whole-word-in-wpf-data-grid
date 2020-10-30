using Syncfusion.UI.Xaml.Grid;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SearchPanel
{
    public class CustomSearchHelper : SearchHelper
    {
        public CustomSearchHelper(SfDataGrid dataGrid) : base(dataGrid)
        {

        }

        
        protected override bool ApplyInline(DataColumnBase column, object data, bool ApplySearchHighlightBrush)
        {
            var tempSearchText = SearchText;
            String[] metaCharacters = { "\\", "^", "$", "{", "}", "[", "]", "(", ")", ".", "*", "+", "?", "|", "<", ">", "-", "&" };
            if (metaCharacters.Any(tempSearchText.Contains))
            {
                for (int i = 0; i < metaCharacters.Length; i++)
                {
                    if (tempSearchText.Contains(metaCharacters[i]))
                        tempSearchText = tempSearchText.Replace(metaCharacters[i], "\\" + metaCharacters[i]);
                }
            }

            string[] substrings;
            Regex regex;

            if (!AllowCaseSensitiveSearch)
                regex = new Regex("^(" + tempSearchText + ")$", RegexOptions.IgnoreCase);
            else
                regex = new Regex("^(" + tempSearchText + ")$", RegexOptions.None);

            FrameworkElement columnElement = (FrameworkElement)column.GetType().GetField("columnElement", System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance).GetValue(column);
            var textBlock = (columnElement as ContentControl).Content as TextBlock;
            textBlock.Inlines.Clear();
            substrings = regex.Split(data.ToString());
            bool success = false;
            foreach (var item in substrings)
            {
                if (regex.Match(item).Success)
                {
                    Run run = new Run(item);
                    if (ApplySearchHighlightBrush || column.ColumnIndex == CurrentRowColumnIndex.ColumnIndex && column.RowIndex == CurrentRowColumnIndex.RowIndex)
                    {
                        if (this.ReadLocalValue(SearchHelper.SearchForegroundHighlightBrushProperty) != DependencyProperty.UnsetValue)
                            run.Foreground = this.SearchForegroundHighlightBrush;
                        run.Background = this.SearchHighlightBrush;
                    }
                    else
                    {
                        if (this.ReadLocalValue(SearchHelper.SearchForegroundBrushProperty) != DependencyProperty.UnsetValue)
                            run.Foreground = this.SearchForegroundBrush;
                        run.Background = this.SearchBrush;
                    }
                    if (column.GridColumn is GridHyperlinkColumn)
                        textBlock.Inlines.Add(new Hyperlink(run));
                    else
                        textBlock.Inlines.Add(run);
                    success = true;
                }
                else
                {
                    if (column.GridColumn is GridHyperlinkColumn)
                        textBlock.Inlines.Add(new Hyperlink(new Run(item)));
                    else
                        textBlock.Inlines.Add(item);
                }
            }
            return success;
        }
    }
}
