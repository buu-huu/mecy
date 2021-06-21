﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MecyInformation
{
    static class XMLParser
    {
        public static List<Mesocyclone> ParseMesos(string path)
        {
            List<Mesocyclone> parsedMesos = new List<Mesocyclone>();

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(path);
            
            foreach (XmlNode node in xdoc.DocumentElement.ChildNodes)
            {
                if (node.Name == "event")
                {
                    Mesocyclone meso = new Mesocyclone();
                    meso.Id = Convert.ToInt32(node.Attributes["ID"].Value);

                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        switch (childNode.Name)
                        {
                            case "time":
                                meso.Time = childNode.InnerText;
                                break;
                            case "location":
                                foreach (XmlNode ellipseNode in childNode.ChildNodes[0].ChildNodes[0])
                                {
                                    if (ellipseNode.Name == "moving-point")
                                    {
                                        foreach (XmlNode movingPointNode in ellipseNode.ChildNodes)
                                        {
                                            if (movingPointNode.Name == "latitude")
                                            {
                                                meso.Latitude = Convert.ToDouble(movingPointNode.InnerText);
                                            }
                                            if (movingPointNode.Name == "longitude")
                                            {
                                                meso.Longitude = Convert.ToDouble(movingPointNode.InnerText);
                                            }
                                            /*
                                            if (movingPointNode.Name == "polar_motion")
                                            {
                                                meso.PolarMotion = Convert.ToDouble(movingPointNode.ChildNodes[0].InnerText);
                                            }
                                            */
                                        }
                                    }
                                    /*
                                    if (ellipseNode.Name == "major_axis")
                                    {
                                        meso.MajorAxis = Convert.ToDouble(ellipseNode.InnerText);
                                    }
                                    if (ellipseNode.Name == "minor_axis")
                                    {
                                        meso.MinorAxis = Convert.ToDouble(ellipseNode.InnerText);
                                    }
                                    if (ellipseNode.Name == "orientation")
                                    {
                                        meso.Orientation = Convert.ToInt32(ellipseNode.InnerText);
                                    }
                                    */
                                }
                                break;
                            case "nowcast-parameters":
                                foreach (XmlNode paramNode in childNode.ChildNodes)
                                {
                                    switch (paramNode.Name)
                                    {
                                        case "mesocyclone_shear_mean":
                                            meso.ShearMean = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_shear_max":
                                            meso.ShearMax = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_momentum_mean":
                                            meso.MomentumMean = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_momentum_max":
                                            meso.MomentumMax = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_diameter":
                                            meso.Diameter = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_diameter_equivalent":
                                            meso.DiameterEquivalent = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_top":
                                            meso.Top = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_base":
                                            meso.MesoBase = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_echotop":
                                            meso.Echotop = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_vil":
                                            meso.Vil = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_shear_vectors":
                                            meso.ShearVectors = Convert.ToInt32(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_shear_features":
                                            meso.ShearFeatures = Convert.ToInt32(paramNode.InnerText);
                                            break;
                                        case "mean_dbz":
                                            meso.MeanDBZ = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "max_dbz":
                                            meso.MaxDBZ = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_velocity_max":
                                            meso.VelocityMax = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_velocity_rotational_max":
                                            meso.VelocityRotationalMax = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_velocity_rotational_mean":
                                            meso.VelocityRotationalMean = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "mesocyclone_velocity_rotational_max_closest_to_ground":
                                            meso.VelocityRotationalMaxClosestToGround = Convert.ToDouble(paramNode.InnerText);
                                            break;
                                        case "meso_intensity":
                                            meso.Intensity = Convert.ToInt32(paramNode.InnerText);
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    parsedMesos.Add(meso);
                }
            }
            return parsedMesos;
        }
    }
}