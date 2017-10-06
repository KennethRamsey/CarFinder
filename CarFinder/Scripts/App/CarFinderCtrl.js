/// <reference path="carfindermodule.ts" />
var CarFinder;
(function (CarFinder) {
    var MainController = (function () {
        function MainController($scope, HCL2, ModalService) {
            this.$scope = $scope;
            this.HCL2 = HCL2;
            this.ModalService = ModalService;
            this.carTable = { from: 0, to: 0, total: 0, cars: [] };
            // parameters used to query the WebAPI.
            this.queryParams = { year: null, make: null, model: null, trim: null, page: 1, sort: null, order: null };
            // init lists for the selects.
            this.makes = [];
            this.models = [];
            this.trims = [];
            // years should only be loaded once.
            this.years = this.HCL2.GetYears();
            // init input selected values.
            this.carYear = null;
            this.carMake = null;
            this.carModel = null;
            this.carTrim = null;
            // SO, mainController is now an object with properties.
            // IF we set a vm in the $scope to the MainController, we will no longer have to type $scope again
            // BUT we do have to remember to use the THIS keyword to use a var AFTER we have set it.
            $scope.vm = this;
            // need to do an initial update to set the make field.
            this.update('makes');
        }
        // update select list values.
        MainController.prototype.update = function (carList) {
            // update query parameters to send to webAPI
            this.updateQueryObject("no sort");
            // the "this" pointer gets lost in the callbacks, need solid reference.
            var ctrl = this;
            // update list if list's name is give, and other tests.            
            // on update of lists, if the previously selected value is not in the list, then set the selected val to null.
            switch (carList) {
                case "makes":
                    this.HCL2.GetMakes(this.queryParams).$promise.then(updateMakes);
                    break;
                case "models":
                    if (this.carMake) {
                        this.HCL2.GetModels(this.queryParams).$promise.then(updateModels);
                    }
                    else {
                        // no car make selected, update values and next field.
                        this.carModel = null;
                        this.update("trims");
                    }
                    break;
                case "trims":
                    if (this.carModel) {
                        this.HCL2.GetTrims(this.queryParams).$promise.then(updateTrims);
                    }
                    else {
                        // no car model, so set trim to null.
                        this.carTrim = null;
                    }
                    // THIS is now the point that I should update the cars listed.
                    this.getCarTable(1);
                    break;
            }
            function updateMakes(results) {
                ctrl.makes = results;
                if (results.indexOf(ctrl.carMake) === -1)
                    ctrl.carMake = null;
                // update next field
                ctrl.update("models");
            }
            function updateModels(results) {
                ctrl.models = results;
                if (results.indexOf(ctrl.carModel) === -1)
                    ctrl.carModel = null;
                // update next field
                ctrl.update("trims");
            }
            function updateTrims(results) {
                ctrl.trims = results;
                if (results.indexOf(ctrl.carTrim) === -1)
                    ctrl.carTrim = null;
            }
        };
        // function to update the car table.
        MainController.prototype.getCarTable = function (page) {
            // function is designed to be called from the switch case "trims" in update,
            // OR by the pager.
            this.updateQueryObject();
            // b/c
            if (!this.carYear && !this.carMake) {
                this.carTable = { from: 0, to: 0, total: 0, cars: [] };
            }
            else {
                // this means that the queryParams object should already contain all of the approptiate values except page.
                this.queryParams.page = page;
                this.carTable = this.HCL2.GetCars(this.queryParams);
            }
        };
        // function to properly handle paging.
        MainController.prototype.newPage = function (to) {
            switch (to) {
                case "Prev":
                    // make sure we don't go before the 1st page.
                    if (this.carTable.from <= 1)
                        return;
                    this.getCarTable(this.queryParams.page - 1);
                    break;
                case "Next":
                    // make sure we don't go past last page.
                    if (this.carTable.to >= this.carTable.total)
                        return;
                    this.getCarTable(this.queryParams.page + 1);
                    break;
            }
        };
        // helper function to be called right before a call to the WebAPI to make sure all parameters are up to date.
        MainController.prototype.updateQueryObject = function (option) {
            // update query parameters to send to webAPI
            this.queryParams = { year: this.carYear, make: this.carMake, model: this.carModel, trim: this.carTrim, page: this.queryParams.page, sort: this.queryParams.sort, order: this.queryParams.order };
            if (option === "no sort") {
                this.queryParams.sort = null;
                this.queryParams.order = null;
            }
        };
        // function to call when a column name is clicked to apply sorting.
        MainController.prototype.sort = function (sortField) {
            // change order if given field is the sort field.
            if (this.queryParams.sort === sortField)
                this.queryParams.order = (this.queryParams.order === "desc") ? "asc" : "desc";
            else {
                this.queryParams.sort = sortField;
                this.queryParams.order = "asc";
            }
            // now update car table.s
            this.getCarTable(1);
        };
        // helper func for page to show arrow showing sorting.
        MainController.prototype.arrow = function (field) {
            // show arrow if field is sorted field.
            if (this.queryParams.sort === field)
                return (this.queryParams.order === "desc") ? " ▲" : " ▼";
        };
        // Function to show the modal!!!
        MainController.prototype.showDetailsModal = function (car) {
            // set car object to the car chosen.
            this.HCL2.car = car;
            this.ModalService.showModal({
                templateUrl: "/Scripts/App/Details.html",
                controller: "ModalController"
            }).then(function (modal) {
                //it's a bootstrap element, use 'modal' to show it
                modal.element.modal();
                modal.close
                    .then(function (result) {
                    console.log(result);
                });
            });
        };
        return MainController;
    }());
    CarFinder.MainController = MainController;
    angular.module("CarFinder").controller("MainCtrl", ["$scope", "HCL2", "ModalService", MainController]);
})(CarFinder || (CarFinder = {}));
//# sourceMappingURL=carfinderctrl.js.map