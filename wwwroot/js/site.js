// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



document.getElementById('searchCategory').addEventListener('change', function () {
    var selectedValue = this.value;
    var urlParams = new URLSearchParams(window.location.search);
    if (selectedValue === 'All') {
        urlParams.set('searchCategory', 'All');
    } else if (selectedValue === 'Active') {
        urlParams.set('searchCategory', 'Active');
    } else if (selectedValue === 'Deactive') {
        urlParams.set('searchCategory', 'Deactive');
    }
    window.location.href = '?' + urlParams.toString();
});


var branchCategory = new URLSearchParams(window.location.search).get("searchCategory");
if (branchCategory) {
    document.getElementById("searchCategory").value = branchCategory;
}

