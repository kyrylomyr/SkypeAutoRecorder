using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SkypeAutoRecorder.Configuration
{
    /// <summary>
    /// Settings filter that provides file name for saving conversations with specified list of contacts.
    /// </summary>
    [Serializable]
    public class Filter : INotifyPropertyChanged
    {
        private string _contacts;
        private string _rawFileName;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public Filter()
        {
            _contacts = string.Empty;
            _rawFileName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the contacts.
        /// </summary>
        /// <value>
        /// The contacts separated with comma or semicolon.
        /// </value>
        [XmlAttribute("contacts")]
        public string Contacts
        {
            get
            {
                return _contacts;
            }
            set
            {
                if (_contacts != value)
                {
                    _contacts = value;
                    onPropertyChanged(new PropertyChangedEventArgs("Contacts"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the file name with placeholders for saving record.
        /// </summary>
        /// <value>
        /// The file name with placeholders.
        /// </value>
        [XmlAttribute("fileName")]
        public string RawFileName
        {
            get
            {
                return _rawFileName;
            }
            set
            {
                if (_rawFileName != value)
                {
                    _rawFileName = value;
                    onPropertyChanged(new PropertyChangedEventArgs("RawFileName"));
                }
            }
        }

        private void onPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
