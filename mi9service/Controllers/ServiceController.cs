using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Cors;
using mi9service.Models;
using System.Net;

namespace mi9service.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ServiceController : ApiController
    {
        /// <summary>
        /// Post action method receives a HttpRequestMessage with a content of type Show and returns HttpResponseMessage with of type Response.
        /// </summary>
        /// <param name="input">JSON input of the type show</param>
        /// <returns>HttpResponseMesage with Response or Error types</returns>
        [HttpPost]
        public HttpResponseMessage Post(Show input)
        {
            if (input == null)
            {
                return Request.CreateResponse<Error>(HttpStatusCode.BadRequest, new Error
                {
                    error = "Could not decode request: JSON parsing failed"
                });
            }
            try
            {
                return Request.CreateResponse<Output>(HttpStatusCode.OK, ShowsWithDrmAndAtleastOneEpisode(input));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse<Error>(HttpStatusCode.BadRequest, new Error
                {
                    error = "Could not decode request: " + ex.Message
                });
            }
        }

        /// <summary>
        /// The cofing challenge logic is implemented in this method.
        /// This method uses LINQ to Object to filter the shows with DRM true and episodeCount > 0
        /// </summary>
        /// <param name="input">A deserialized object of shows  equivalent to the input JSON.</param>
        /// <returns>An object of type Ouptput with a property of response of image, slug and title fields from the input.</returns>
        private Output ShowsWithDrmAndAtleastOneEpisode(Show input)
        {
            var data = (from p in input.payload
                        where
                            p.drm &&
                            p.episodeCount > 0
                        select new Response
                        {
                            image = p.image.showImage,
                            slug = p.slug,
                            title = p.title
                        }
                        ).ToList();
            return new Output
            {
                response = data
            };
        }

    }
}
