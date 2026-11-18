﻿var dataTable;

$(document).ready(function () {
    // This code is used to determine which status of orders to load based on the query string in the URL. 
    // It checks for specific keywords in the URL and calls the loadDataTable function with the corresponding status. 
    // If no specific status is found, it defaults to loading all orders.
    var url = window.location.search;
    if (url.includes("cancelled")) {
        loadDataTable("cancelled");
    } else if (url.includes("shipped")) {
        loadDataTable("shipped");
    } else if (url.includes("pending")) {
            loadDataTable("pending");
    } else if (url.includes("processing")) {
        loadDataTable("processing");
    } else if (url.includes("approved")) {
        loadDataTable("approved");
    } else {
        loadDataTable("all");
    }
    
})

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        ajax: '/admin/order/getall?status=' + status,
        columns: [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "20%" },
            {
                data: 'orderStatus', "width": "15%", "render": function (data) {
                    var bg = '#f1f5f9', fg = '#475569';
                    if (data === 'Approved') { bg = '#d1fae5'; fg = '#047857'; }
                    else if (data === 'Processing') { bg = '#fef3c7'; fg = '#b45309'; }
                    else if (data === 'Shipped') { bg = '#dbeafe'; fg = '#1d4ed8'; }
                    else if (data === 'Cancelled') { bg = '#ffe4e6'; fg = '#be123c'; }
                    else if (data === 'Refunded') { bg = '#fce7f3'; fg = '#9f1239'; }
                    return '<span style="display:inline-flex;align-items:center;font-size:12px;font-weight:500;padding:3px 10px;border-radius:4px;background:' + bg + ';color:' + fg + '">' + data + '</span>';
                }
            },

            { data: 'orderTotal', "width": "15%", "render": function (data) { return '$' + data.toFixed(2); } },
            {
                data: 'id', "width": "10%", "render": function (data) {
                    return `<div class="d-flex gap-2 justify-content-end">
                            <a href="/admin/order/details?orderId=${data}" class="btn btn-sm btn-outline-success">
                                 <i class="bi bi-pencil-square"></i> Details
                            </a>
                             
                        </div > `;
                }
            }
        ]
    });

}