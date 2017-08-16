using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using rrd4n.Common;

namespace rrd4n.Core
{
   public class XmlImporter : DataImporter
   {
      XmlDataDocument dataDocument = new XmlDataDocument();
      public XmlImporter(string xmlFilePath)
      {
         dataDocument.LoadXml(xmlFilePath);
      //root = Util.Xml.getRootElement(new File(xmlFilePath));
      //dsNodes = Util.Xml.getChildNodes(root, "ds");
      //arcNodes = Util.Xml.getChildNodes(root, "rra");
	}
      private long ReadLong(string xPath)
      {
         XmlNode node = dataDocument.SelectSingleNode(xPath);
         return long.Parse(node.Value);
      }
      public override string getVersion()
      {
         return dataDocument.SelectSingleNode("//rrd/version").Value;
      }

      public override long getLastUpdateTime()
      {
         return ReadLong("//rrd/lastupdate");
      }

      public override long getStep()
      {
         return ReadLong("//rrd/step");
      }

      public override int getDsCount()
      {
         return dataDocument.SelectNodes("//rrd/ds").Count;
      }

      public override int getArcCount()
      {
         return dataDocument.SelectNodes("//rrd/rra").Count;
      }

      public override string getDsName(int dsIndex)
      {
         XmlNodeList nodes = dataDocument.SelectNodes("//rrd/ds");
         return nodes[dsIndex].SelectSingleNode("name").Value;
      }

      public override string getDsType(int dsIndex)
      {
         throw new System.NotImplementedException();
      }

      public override long getHeartbeat(int dsIndex)
      {
         throw new System.NotImplementedException();
      }

      public override double getMinValue(int dsIndex)
      {
         throw new System.NotImplementedException();
      }

      public override double getMaxValue(int dsIndex)
      {
         throw new System.NotImplementedException();
      }

      public override double getLastValue(int dsIndex)
      {
         throw new System.NotImplementedException();
      }

      public override double getAccumValue(int dsIndex)
      {
         throw new System.NotImplementedException();
      }

      public override long getNanSeconds(int dsIndex)
      {
         throw new System.NotImplementedException();
      }

      public override ConsolFun getConsolFun(int arcIndex)
      {
         throw new System.NotImplementedException();
      }

      public override double getXff(int arcIndex)
      {
         throw new System.NotImplementedException();
      }

      public override int getSteps(int arcIndex)
      {
         throw new System.NotImplementedException();
      }

      public override int getRows(int arcIndex)
      {
         throw new System.NotImplementedException();
      }

      public override double getStateAccumValue(int arcIndex, int dsIndex)
      {
         throw new System.NotImplementedException();
      }

      public override int getStateNanSteps(int arcIndex, int dsIndex)
      {
         throw new System.NotImplementedException();
      }

      public override double[] getValues(int arcIndex, int dsIndex)
      {
         throw new System.NotImplementedException();
      }
   }
}
