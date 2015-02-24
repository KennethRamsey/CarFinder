/// <reference path="carfindermodule.ts" />


module CarFinder {

    // interface for object to be passed to the query
    export interface ICarQuery {
        id?: number;
        year?: number;
        make?: string;
        model?: string;
        sort?: string;
        order?: string;
    }

    export class HCL2 {

        // by using get, we will return a function with an interface, but now we will have a proper typescript interface for it.
        constructor(
            private $resource: ng.resource.IResourceService,
            public car: Object // this will be a car that will allow passing between controllers.
            ) { }

        // interface for HCL2
        GetCars(queryObject: ICarQuery) {
            return this.$resource("/api/cars").get(queryObject);
        }

        GetCarById(queryObject: ICarQuery) {
            return this.$resource("/api/cars/:id", { id: "@id" }).query(queryObject);
        }

        GetYears() {
            return this.$resource("/api/cars/years").query();
        }

        GetMakes(queryObject: ICarQuery) {
            return this.$resource("/api/cars/makes/:year", { year: "@year" }).query(queryObject);
        }

        GetModels(queryObject: ICarQuery) {
            return this.$resource("/api/cars/models/:make/:year", { make: "@make", year: "@year" }).query(queryObject);
        }

        GetTrims(queryObject: ICarQuery) {
            return this.$resource("/api/cars/trims/:make/:model/:year", { make: "@make", model: "@model", year: "@year" }).query(queryObject);
        }
    }


    angular.module("CarFinder").service("HCL2", ["$resource", HCL2]);
}