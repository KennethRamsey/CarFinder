using CarFinder.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;


namespace CarFinder.Controllers
{
    /// <summary>
    /// Controller to get all car info from our system.
    /// </summary>
    [RoutePrefix("api/cars")]
    public class CarController : ApiController
    {
        /// signature: api/cars
        /// 
        /// <summary>
        /// Function that returns all of the cars the fit the given paramters.
        /// at least one of the params year, make, model must be given.
        /// trim and page are optional
        /// </summary>        
        [Route("")]
        public IHttpActionResult GetCars(
              int? year = null,
            string make = null,
            string model = null,
            string trim = null,
              int? page = 1,
            string sort = null,
            string order = null)
        {
            // create and open a new connection object
            using (SqlConnection conn = DB.GetOpenConnection())
            {
                // create a comand object identifying the stored procedure
                SqlCommand cmd = DB.StoredProc("GetCars", conn);

                void addParam<T>(string name, T val) => cmd.Parameters.Add(new SqlParameter(name, val));

                // add filtering paramaters to command
                if (year != null) addParam("@year", year);
                if (make != null) addParam("@make", make);
                if (model != null) addParam("@model", model);
                if (trim != null) addParam("@trim", trim);
                if (sort != null) addParam("@sort", sort);
                if (order != null) addParam("@order", order);

                var badPage = page < 1;
                if (badPage) page = 1;


                cmd.Parameters.Add(new SqlParameter("@page", page));


                SqlParameter addOutputParam(string name)
                {
                    var param = new SqlParameter(name, 0);
                    param.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(param);

                    return param;
                }

                // add output params to command
                var from = addOutputParam("@from");
                var to = addOutputParam("@to");
                var total = addOutputParam("@total");

                // execute the command
                SqlDataReader rdr = cmd.ExecuteReader();

                // create empty list of cars and find the indexes of the car's properties.
                var cars = new List<Car>();
                Car.SetIndexes(rdr);

                // populate cars list
                while (rdr.Read())
                    cars.Add(new Car(rdr));

                // so you HAVE to close the connection to get the values from your output parameter. (but why?)_
                conn.Close();

                // This functions needs to also return some info, like what cars out of the total are being returned,
                // SO angular will know when to continue paging and when to stop.
                var carSet = Json(new
                {
                    from = from.Value,
                    to = to.Value,
                    total = total.Value,
                    cars = cars
                });

                return carSet;
            }
        }


        /// signature: api/cars/{id}
        /// 
        /// <summary>
        /// Get car with a given id.
        /// </summary>
        [Route("{id:int}")]
        public IHttpActionResult GetCarById(int id)
        {
            using (SqlConnection conn = DB.GetOpenConnection())
            {
                // get car from db.
                SqlCommand sqlGetCarById = DB.StoredProc("GetCarById", conn);
                sqlGetCarById.Parameters.Add(new SqlParameter("@id", id));

                SqlDataReader rdr = sqlGetCarById.ExecuteReader();

                // get car, make sure that it exists.

                if (rdr.Read())
                {
                    string Get(string s) => rdr[s].ToString();

                    Car car = new Car
                    {
                        Id = Get("id"),
                        Year = Get("year"),
                        Make = Get("make"),
                        Model = Get("model"),
                        Trim = Get("trim"),
                        TopSpeed = Get("top_speed"),
                        TransmissionType = Get("transmission_type"),
                        Seats = Get("seats"),
                        Doors = Get("doors"),
                        DriveType = Get("drive_type"),
                    };

                    return Ok(car);
                }

                return BadRequest();
            }
        }

        /// signature: api/cars/years
        /// 
        /// <summary>
        /// Get a list of years for cars in HCL2.
        /// </summary>
        [Route("years")]
        public IHttpActionResult GetYears()
        {
            using (SqlConnection conn = DB.GetOpenConnection())
            {
                // get years from database
                SqlCommand sqlGetYears = DB.StoredProc("GetYears", conn);

                SqlDataReader rdr = sqlGetYears.ExecuteReader();

                // put years in list
                List<string> years = rdr.RdrToList();

                // return years
                return Ok(years);
            }
        }

        /// Signature: api/{make}/{year?}
        /// 
        /// <summary>
        /// get a list of car makes. Optionally filter makes in a given year.
        /// </summary>
        [Route("makes/{year:int?}")]
        public IHttpActionResult GetMakes(int? year = null)
        {
            using (SqlConnection conn = DB.GetOpenConnection())
            {
                // get makes from db.
                SqlCommand sqlGetMakes = DB.StoredProc("GetMakes", conn);

                if (year.HasValue)
                    sqlGetMakes.Parameters.Add(new SqlParameter("@year", year));

                SqlDataReader rdr = sqlGetMakes.ExecuteReader();

                // put makes in list
                List<string> makes = rdr.RdrToList();

                return Ok(makes);
            }
        }

        /// Signature: api/models/{make}/{year?}
        /// 
        /// <summary>
        /// Gets a list of models for a given make of a car, optional filtering on a year.
        /// </summary>
        [Route("models/{make}/{year:int?}")]
        public IHttpActionResult GetModels(string make, int? year = null)
        {
            using (SqlConnection conn = DB.GetOpenConnection())
            {
                // get data from database.
                SqlCommand sqlGetModels = DB.StoredProc("GetModels", conn);

                sqlGetModels.Parameters.Add(new SqlParameter("@make", make));
                if (year.HasValue)
                    sqlGetModels.Parameters.Add(new SqlParameter("@year", year));

                SqlDataReader rdr = sqlGetModels.ExecuteReader();

                // convert data to list.
                List<string> models = rdr.RdrToList();

                return Ok(models);
            }
        }

        /// Signature: api/cars/trims/{make}/{model}/{year?}
        /// 
        /// <summary>
        /// get a list of trims for a given car make and model, optionally filter on year.
        /// </summary>        
        [Route("trims/{make}/{model}/{year:int?}")]
        public IHttpActionResult GetTrims(string make, string model, int? year = null)
        {
            using (SqlConnection conn = DB.GetOpenConnection())
            {
                SqlCommand sqlGetTrims = DB.StoredProc("GetTrims", conn);

                sqlGetTrims.Parameters.Add(new SqlParameter("@make", make));
                sqlGetTrims.Parameters.Add(new SqlParameter("@model", model));
                if (year.HasValue) sqlGetTrims.Parameters.Add(new SqlParameter("@year", year));

                SqlDataReader rdr = sqlGetTrims.ExecuteReader();

                List<string> trims = rdr.RdrToList();

                return Ok(trims);
            }
        }

    }


}
