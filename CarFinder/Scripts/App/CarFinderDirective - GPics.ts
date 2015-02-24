/// <reference path="carfindermodule.ts" />

declare var google;


module CarFinder {

    // SO, this loads the image search functionality(version 1)
    // this needs to only run ONCE, so we want it in the directive file, BUT not in the directive.
    google.load("search", "1");


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

                // Create an Image Search instance.
                var imageSearch = new google.search.ImageSearch();

                // Set searchComplete as the callback function when a search is copmlete.
                imageSearch.setSearchCompleteCallback(this, function () {

                    // Check that we got results
                    if (imageSearch.results && imageSearch.results.length > 0) {

                        // set src on img tags to 1st result.
                        // 1st img is the thumbnail, 2nd is the hover over large image.
                        var imgs = element.find("img");
                        (<HTMLImageElement>imgs[0]).src = imageSearch.results[0].tbUrl;
                        (<HTMLImageElement>imgs[1]).src = imageSearch.results[0].url;
                        (<HTMLImageElement>imgs[1]).style.backgroundColor = "white";

                        // store car url on car.
                        scope.car.url = imageSearch.results[0].url;

                        // 
                        // NEED THE GOOGLE BRANDING IF I WANT TO PUT THIS ONLINE!
                        //
                    }
                });

                // Find me a beautiful car.
                imageSearch.execute(scope.searchTerm);
             }
        }
    }

    angular.module("CarFinder").directive("googleThumbnail", [GoogleImages]);
}