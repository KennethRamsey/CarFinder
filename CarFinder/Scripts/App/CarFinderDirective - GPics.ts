/// <reference path="carfindermodule.ts" />

module CarFinder {

    function GoogleImages() {

        return {

            scope: {
                searchTerm: "@searchTerm", // the @ means the var will be accessed as a string.
                car: "="
            },

            template: '<div class="thumbnail">'
            + '    <img alt="" />'
            + '    <span><img alt="{{searchTerm}}" /></span>'
            + '</div>',

            replace: true,

            link: function (scope, element: JQuery) {

                // working flickr site http://justcats.applicate.de/


                var baseUrl = "https://api.flickr.com/services/rest/?method=flickr.photos.search";
                var query = "&api_key=&content_type=1&per_page=1&text=";

                var ajaxOptions = {
                    url: baseUrl + query + scope.searchTerm,
                    method: "get",
                    success: showImage,
                    error: function () { console.log("error", arguments); }
                };

                $.ajax(ajaxOptions);


                function showImage(result) {

                    var url = "https://www.pexels.com/photo/road-yellow-model-car-97353/"; // default car pic.

                    if (ensureGood(result)) {
                        var photos = result.documentElement.children[0].children;
                        var url = GetPhotoUrl(photos[0]);
                    }


                    // set src on img tags to 1st result.
                    // 1st img is the thumbnail, 2nd is the hover over large image.
                    var imgs = element.find("img");
                    (<HTMLImageElement>imgs[0]).src = url; //imageSearch.results[0].tbUrl;
                    (<HTMLImageElement>imgs[1]).src = url; //imageSearch.results[0].url;
                    (<HTMLImageElement>imgs[1]).style.backgroundColor = "white";

                    // store car url on car.
                    scope.car.url = url; //imageSearch.results[0].url;
                }


                function GetPhotoUrl(photo) {

                    function get(s) { return photo.attributes.getNamedItem(s).value; }

                    return [
                        "https://farm", get("farm"),
                        ".staticflickr.com/", get("server"),
                        "/", get("id"), "_", get("secret"), ".jpg"].join("");
                }


                function ensureGood(rez) {
                    // rsp has stat ok.
                    var ok = rez.documentElement.attributes.item("stat").value === "ok";
                    if (!ok) {
                        console.log("not ok", rez);
                        return false;
                    }

                    // is photos
                    var photos = rez.documentElement.children[0];

                    var good = photos.children.length > 0;
                    if (!good) {
                        console.log("no photos", rez);
                        return false;
                    }

                    return true;
                }
            }
        }
    }

    angular.module("CarFinder").directive("googleThumbnail", [GoogleImages]);
}