using System.Collections.Generic;
using System.Xml.Linq;

// Courtesy of http://dotnetbyexample.blogspot.com/2011/03/easy-access-to-wmappmanifestxml-app.html
namespace CrossGIS.Core.Utils
{
    /// <summary>
    /// A helper class to easily retrieve data from the WMAppManifest.xml
    /// App tag
    /// </summary>
    public static class ManifestAppInfo
    {
        static Dictionary<string, string> _properties;

        static Dictionary<string, string> Properties
        {
            get
            {
                if (null == _properties)
                {
                    _properties = new Dictionary<string, string>();
                    var appManifestXml = XDocument.Load("WMAppManifest.xml");
                    using (var rdr = appManifestXml.CreateReader(ReaderOptions.None))
                    {
                        rdr.ReadToDescendant("App");
                        if (!rdr.IsStartElement())
                        {
                            throw new System.FormatException(
                               "App tag not found in WMAppManifest.xml ");
                        }
                        rdr.MoveToFirstAttribute();
                        while (rdr.MoveToNextAttribute())
                        {
                            _properties.Add(rdr.Name, rdr.Value);
                        }
                    }
                }
                return _properties;
            }
        }

        public static string Version
        {
            get { return Properties["Version"]; }
        }

        public static string ProductId
        {
            get { return Properties["ProductID"]; }
        }

        public static string Title
        {
            get { return Properties["Title"]; }
        }

        public static string TitleUc
        {
            get
            {
                return !string.IsNullOrEmpty(Title) ?
                         Title.ToUpperInvariant() : null;
            }
        }

        public static string Genre
        {
            get { return Properties["Genre"]; }
        }

        public static string Description
        {
            get { return Properties["Description"]; }
        }

        public static string Publisher
        {
            get { return Properties["Publisher"]; }
        }

        /// <summary>
        /// More info here : http://atomaras.wordpress.com/2012/11/18/wp7-app-on-wp8-breaking-changes-part-1-webrequests-referer-changes/
        /// </summary>
        public static string DefaultRefererHeader
        {
            get
            {
                // WP7 Targeting App -> Running on WP7 device
                //  Both BitmapImage and WebClient send this referer by default
                //  WebClient & BitmapImage : "file:///Applications/Install/{ProductId}/Install/"

                // WP7 Targeting App -> Running on WP8 device
                //  (MS BUG) Referers sent differ
                //  WebClient : "file:///Applications/Install/{ProductId}/Install/"
                //  BitmapImage : "file:///C:/Data/Programs/%7{ProductId}%7D/Install/"

                // WP8 Targeting App -> Running on WP8 device
                //  (MS Issue) Change in behavior. WebClient now does not set a referer by default! 
                //  WebClient : <none>
                //  BitmapImage : "file:///C:/Data/Programs/%7{ProductId}%7D/Install/"

                // In order for all requests to authenticate correctly for all scenarios and 
                // taking into account that the server does a "contains" on the referers then
                // we need to use only the common part (the least common denominator) which is the ProductId
                // as the Referer throughout AND inject the same referer on all WebRequests
                return ProductId.Trim('{', '}').ToUpper();


            }
        }
    }
}

