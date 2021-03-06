function confirmDeletion(name)
{
    return confirm(`Are you sure you want to delete ${name}?`);
}

function confirmUpdate(name) {
    return confirm(`Are you sure you want to update ${name}?`);
}

function confirmOut() {
    return confirm("Are you sure you want to out?");
}

let DataSourceSelect = document.getElementById("data-source-select");

DataSourceSelect.addEventListener('change', (e) => {
    console.log(e.target.value)
    document.cookie = "DataSource=" + e.target.value
    document.location.reload();
});