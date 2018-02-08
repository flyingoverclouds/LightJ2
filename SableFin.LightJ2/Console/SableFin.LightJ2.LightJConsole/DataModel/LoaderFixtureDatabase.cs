using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
    public class LoaderFixtureDatabase
    {
        public static FixtureDatabase LoadFromXml(XElement xeFixtureDatabase)
        {
            if (xeFixtureDatabase.Name.LocalName != "FixturesDatabase")
            {
                throw new ArgumentException("root element is not 'FixtureDatabase'");
            }
            var db = new FixtureDatabase();
           
            try
            {
                db.Name = xeFixtureDatabase.Attribute("name").Value;
                db.Date = xeFixtureDatabase.Attribute("date").Value;
                db.Author = xeFixtureDatabase.Attribute("author").Value;
                db.Origin = xeFixtureDatabase.Attribute("origin").Value;
                db.Version = xeFixtureDatabase.Attribute("version").Value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                // TODO add trace load failed
            }
            int fixturePos = 0;
            foreach (XElement xeFixt in xeFixtureDatabase.Elements("Fixture"))
            {
                fixturePos++;
                try {
                    var f = LoaderFixture.LoadFromXml(xeFixt);
                    db.Fixtures.Add(f.Id, f);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("Unable to load fixture number " + fixturePos.ToString() + " : " + ex.ToString());
                }
                
            }
            return db;

        }
    }
}
