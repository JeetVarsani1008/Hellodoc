//this ajax is call when this page is load
$(document).ready(function () {

    $.ajax({
        url: "/Provider/ProviderDashboardTable",
        type: "POST",
        dataType: "html",
        success: function (data) {

            $("#provider-partial-table").html(data);

        },
        error: function () {
            $("#provider-partial-table").html('Something Went Wrong');
        }
    });

});


function fetchRequestsProvider(statusarray, reqtypeid, searchdata) {
    console.log(reqtypeid)
    console.log("inside fetchreq");
    // var status = status;
    var searchdata = $('#searchdata').val().trim();
    var statusarray = statusarray;
    var reqtypeid = reqtypeid;


     $.ajax({
        method: "GET",
        url: "/Provider/FetchRequestsProvider",
        data: {
            statusarray: statusarray,
            reqtypeid: reqtypeid,
            searchdata: searchdata,
        },
        success: function (response) {
            console.log(status);
            console.log("Function Success");
            $('#provider-partial-table').html(response);
            $('#searchdata').val("");
        },
        error: function () {
            $('#provider-partial-table').html("Something Went Wrong");
        }
    })
};

function filterRequestsProvider(statusarray, reqtypeid, searchdata) {
    console.log(reqtypeid)
    console.log("inside fetchreq");
    // var status = status;
    var searchdata = $('#searchdata').val().trim();
    var statusarray = statusarray;
    var reqtypeid = reqtypeid;


     $.ajax({
        method: "GET",
        url: "/Provider/FilterRequestsProvider",
        data: {
            statusarray: statusarray,
            requestTypeId: reqtypeid,
            searchdata: searchdata,
        },
        success: function (response) {
            console.log(status);
            console.log("Function Success");
            $('#provider-partial-table').html(response);
            $('#searchdata').val("");
        },
        error: function () {
            $('#provider-partial-table').html("Something Went Wrong");
        }
    })
};


function providersendlink() {
    $.ajax({
        url: "/Provider/SendLink",
        type: "GET",
        dataType : "html",
        success: function (data) {
            $('#providerSendlink').html(data);
            $('#sendlinkmodel').modal("show");
        },
        error: function () {
            $('#providerSendlink').html("Something Went Wrong");
        }
    })
}
