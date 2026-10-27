$('#tblData').DataTable({
    ajax: '/product/getall',
    columns: [
        { data: 'title' },
        { data: "description" },
        { data: "isbn" },
        { data: "author" },
        { data: "listPrice" },
        { data: "price" },
        { data: "price50" },
        { data: "price100" },
        { data: "categoryId" },
        { data: "category" },
        { data: "imageUrl" },
    ]
})

