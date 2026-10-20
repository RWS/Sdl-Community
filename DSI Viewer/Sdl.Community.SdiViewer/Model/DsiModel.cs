using System.Linq;

namespace Sdl.Community.DsiViewer.Model
{
	public class DsiModel : ModelBase
    {
        private string _code;
        private string _description;
        private string _displayName;
        private string _rowColor;

        public string Code
        {
            get => _code;
            set
            {
                if (_code == value)
                {
                    return;
                }
                _code = value;
                OnPropertyChanged(nameof(Code));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description == value)
                {
                    return;
                }
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName == value)
                {
                    return;
                }
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

        /// <summary>One-line description for screen readers, e.g. "P, Paragraph, A paragraph of text".</summary>
        public string Summary => string.Join(", ", new[] { Code, DisplayName, Description?.Trim() }.Where(part => !string.IsNullOrWhiteSpace(part)));

        public string RowColor
        {
            get => _rowColor;
            set
            {
                if (_rowColor == value)
                {
                    return;
                }
                _rowColor = value;
                OnPropertyChanged(nameof(RowColor));
            }
        }
    }
}