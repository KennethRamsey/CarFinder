/// <reference path="carfindermodule.ts" />

var CarFinder;
(function (CarFinder) {
    function GoogleImages() {
        var gpics = {};

        // load google.
        google.load('search', '1');

        // vars to hold s
        var imageSearch = new Array();

        var allSearches = $("td[data-search]");

        function searchComplete(i) {
            // Check that we got results
            if (imageSearch[i].results && imageSearch[i].results.length > 0) {
                // element to append an img to.
                var contentDiv = allSearches[i];

                // Get 1st result.
                var result = imageSearch[i].results[0];

                // must append children of this format.
                //<div class="thumbnail">s
                //    <img src="@Url.Content(car.PicUrl)" alt="" />
                //    <span><img src="@Url.Content(car.PicUrl)" alt="" /></span>
                //</div>
                var div1 = document.createElement('div');
                div1.className = "thumbnail";

                var img1 = document.createElement('img');
                img1.src = result.url;

                var span1 = document.createElement('span');
                var img2 = document.createElement('img');
                img2.src = result.url;

                //img2.attributes["style"].value = "width: 500px;";
                span1.appendChild(img2);
                div1.appendChild(img1);
                div1.appendChild(span1);

                // Put our image in the content
                contentDiv.appendChild(div1);
            }
        }

        function OnLoad() {
            // need to refresh the elements to update everytime we call the OnLoad functions.
            allSearches = $("td[data-search]");

            for (var i = 0; i < allSearches.length; i++) {
                // Create an Image Search instance.
                imageSearch[i] = new google.search.ImageSearch();

                // Set searchComplete as the callback function when a search is copmlete.
                imageSearch[i].setSearchCompleteCallback(this, searchComplete, [i]);

                var searchTerm = allSearches[i].attributes["data-search"].value;

                // Find me a beautiful car.
                imageSearch[i].execute(searchTerm);
            }
            // Include the required Google branding
            //google.search.Search.getBranding('branding');
        }

        google.setOnLoadCallback(OnLoad);

        return gpics;
    }

    angular.module("CarFinder").directive("gPics", [GoogleImages]);
})(CarFinder || (CarFinder = {}));
//# sourceMappingURL=CarFinderSvc - GPics.js.map
