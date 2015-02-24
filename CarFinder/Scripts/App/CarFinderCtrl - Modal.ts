/// <reference path="carfindermodule.ts" />


module CarFinder {

    angular
        .module("CarFinder")
        .controller('ModalController', ['$scope', 'close', 'HCL2', function ($scope, close, HCL2) {

            $scope.car = HCL2.car;

            $scope.close = function (result) {
                close(result, 500); // close, but give 500ms for bootstrap to animate
            };
        }]);

}
