
/// <reference path="carfindermodule.ts" />


module CarFinder {

    function NoAnimation($animate) {

        return {
            restrict: 'A',

            link: function (scope, element: JQuery) {
                $animate.enabled(false, element);
            }
        };
    }

    angular.module("CarFinder").directive('noAnimate', ['$animate', NoAnimation]);
}