using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CarFinder.Models
{
    [DataContract]
    class Car
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Year { get; set; }
        [DataMember]
        public string Make { get; set; }
        [DataMember]
        public string Model { get; set; }
        [DataMember]
        public string Trim { get; set; }
        [DataMember]
        public string TopSpeed { get; set; }
        [DataMember]
        public string Seats { get; set; }
        [DataMember]
        public string Doors { get; set; }
        [DataMember]
        public string DriveType { get; set; }
        [DataMember]
        public string TransmissionType { get; set; }


        public Car() { }

        // private properties to safely create cars from reader.
        private static int idIndex { get; set; }
        private static int yearIndex { get; set; }
        private static int makeIndex { get; set; }
        private static int modelIndex { get; set; }
        private static int trimIndex { get; set; }
        private static int topSpeedIndex { get; set; }
        private static int seatsIndex { get; set; }
        private static int doorsIndex { get; set; }
        private static int driveTypeIndex { get; set; }
        private static int transmissionTypeIndex { get; set; }


        /// <summary>
        /// func to set private fields for creation of several cars through the sqldatareadr ctor.
        /// </summary>
        public static void SetIndexes(SqlDataReader rdr)
        {
            idIndex = rdr.GetOrdinal("id");
            yearIndex = rdr.GetOrdinal("year");
            makeIndex = rdr.GetOrdinal("make");
            modelIndex = rdr.GetOrdinal("model");
            trimIndex = rdr.GetOrdinal("trim");
            topSpeedIndex = rdr.GetOrdinal("top_speed");
            seatsIndex = rdr.GetOrdinal("seats");
            doorsIndex = rdr.GetOrdinal("doors");
            driveTypeIndex = rdr.GetOrdinal("drive_type");
            transmissionTypeIndex = rdr.GetOrdinal("transmission_type");
        }


        /// <summary>
        /// ctor to create a car from a sqldatareader. Requires that you previously ran the SetIndexes function.
        /// </summary>
        public Car(SqlDataReader rdr)
        {
            string Get(int i) => rdr[i].ToString() ?? "Not Available";

            Id = Get(idIndex);
            Year = Get(yearIndex);
            Make = Get(makeIndex);
            Model = Get(modelIndex);
            Trim = Get(trimIndex);
            TopSpeed = Get(topSpeedIndex);
            Seats = Get(seatsIndex);
            Doors = Get(doorsIndex);
            DriveType = Get(driveTypeIndex);
            TransmissionType = Get(transmissionTypeIndex);
        }
    }
}