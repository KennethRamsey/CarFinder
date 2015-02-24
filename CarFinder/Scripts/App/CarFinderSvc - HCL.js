/// <reference path="carfindermodule.ts" />
var CarFinder;
(function (CarFinder) {
    

    var HCL2 = (function () {
        // by using get, we will return a function with an interface, but now we will have a proper typescript interface for it.
        function HCL2($resource, car) {
            this.$resource = $resource;
            this.car = car;
        }
        // interface for HCL2
        HCL2.prototype.GetCars = function (queryObject) {
            return this.$resource("/api/cars").get(queryObject);
        };

        HCL2.prototype.GetCarById = function (queryObject) {
            return this.$resource("/api/cars/:id", { id: "@id" }).query(queryObject);
        };

        HCL2.prototype.GetYears = function () {
            return this.$resource("/api/cars/years").query();
        };

        HCL2.prototype.GetMakes = function (queryObject) {
            return this.$resource("/api/cars/makes/:year", { year: "@year" }).query(queryObject);
        };

        HCL2.prototype.GetModels = function (queryObject) {
            return this.$resource("/api/cars/models/:make/:year", { make: "@make", year: "@year" }).query(queryObject);
        };

        HCL2.prototype.GetTrims = function (queryObject) {
            return this.$resource("/api/cars/trims/:make/:model/:year", { make: "@make", model: "@model", year: "@year" }).query(queryObject);
        };
        return HCL2;
    })();
    CarFinder.HCL2 = HCL2;

    angular.module("CarFinder").service("HCL2", ["$resource", HCL2]);
})(CarFinder || (CarFinder = {}));
//# sourceMappingURL=CarFinderSvc - HCL.js.map
