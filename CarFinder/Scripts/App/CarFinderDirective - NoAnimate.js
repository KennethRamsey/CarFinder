/// <reference path="carfindermodule.ts" />
var CarFinder;
(function (CarFinder) {
    function NoAnimation($animate) {
        return {
            restrict: 'A',
            link: function (scope, element) {
                $animate.enabled(false, element);
            }
        };
    }
    angular.module("CarFinder").directive('noAnimate', ['$animate', NoAnimation]);
})(CarFinder || (CarFinder = {}));
//# sourceMappingURL=CarFinderDirective - NoAnimate.js.map