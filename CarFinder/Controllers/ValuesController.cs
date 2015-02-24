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
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <param name="trim"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [Route("")]
        public IHttpActionResult GetCars(int? year = null, string make = null, string model = null, string trim = null, int? page = 1, string sort = null, string order = null) 
        {
            // create and open a new connection object
            using (SqlConnection conn = DB.GetOpenConnection())
            {
                // create a comand object identifying the stored procedure
                SqlCommand cmd = DB.StoredProc("GetCars", conn);

                // add filtering paramaters to command
                if (year != null) cmd.Parameters.Add(new SqlParameter("@year", year));
                if (make != null) cmd.Parameters.Add(new SqlParameter("@make", make));
                if (model != null) cmd.Parameters.Add(new SqlParameter("@model", model));
                if (trim != null) cmd.Parameters.Add(new SqlParameter("@trim", trim));
                if (sort != null) cmd.Parameters.Add(new SqlParameter("@sort", sort));
                if (order != null) cmd.Parameters.Add(new SqlParameter("@order", order));

                // CHECK page input!
                if (page < 1) page = 1;
                cmd.Parameters.Add(new SqlParameter("@page", page));


                // add output params to command
                var from = new SqlParameter("@from", 0);
                from.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(from);

                var to = new SqlParameter("@to", 0);
                to.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(to);

                var total = new SqlParameter("@total", 0);
                total.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(total);
                
                // execute the command
                SqlDataReader rdr = cmd.ExecuteReader();

                // create empty list of cars and find the indexes of the car's properties.
                var cars = new List<Car>();
                Car.SetIndexes(rdr);

                // populate cars list
                while (rdr.Read())
                    cars.Add(new Car(rdr));

                // so you HAVE to close the connection to get the values from your output parameter. (BUT WHY??)
                conn.Close();
                
                // This functions needs to also return some info, like what cars out of the total are being returned,
                // SO angular will know when to continue pageing and when to stop.
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
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id:int}")]
        public IHttpActionResult GetCarById(int id)
        {
            using(SqlConnection conn = DB.GetOpenConnection())
	        {
                // get car from db.
		        SqlCommand sqlGetCarById = DB.StoredProc("GetCarById", conn);
                sqlGetCarById.Parameters.Add(new SqlParameter("@id", id));

                SqlDataReader rdr = sqlGetCarById.ExecuteReader();

                // get car, make sure that it exists.

                if (rdr.Read())
                {
                    Car car = new Car {
                        Id = rdr["id"].ToString(),
                        Year = rdr["year"].ToString(),
                        Make = rdr["make"].ToString(),
                        Model = rdr["model"].ToString(),
                        Trim = rdr["trim"].ToString(),
                        TopSpeed = rdr["top_speed"].ToString(),
                        TransmissionType = rdr["transmission_type"].ToString(),
                        Seats = rdr["seats"].ToString(),
                        Doors = rdr["doors"].ToString(),
                        DriveType = rdr["drive_type"].ToString()
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
        /// <returns></returns>
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

                if(year.HasValue)
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
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <returns></returns>
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
        /// <param name="model"></param>
        /// <returns></returns>
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
