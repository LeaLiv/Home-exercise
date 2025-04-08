const uri = 'https://localhost:7085/api';
const statusList = ["ממתינה", "בתהליך", "הושלמה"];

const changeStatus = async (order) => {
    let status = order.querySelector('nav');
    let changeStatusButton = order.querySelector('button');
    status.innerText = statusList[(statusList.indexOf(status.innerText) + 1) % statusList.length];
    if (status.innerText != "ממתינה") {
        changeStatusButton.setAttribute('disabled', true);
    }
    console.log(JSON.stringify({ "status": status.innerText }));
    
    let orderId = order.id.split('_')[1];
    await fetch(`${uri}/order/UpdateOrderStatus/${orderId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ "status": status.innerText })
    }).then(response => console.log("succeed"))   
    .catch(error => console.error('Unable to update item.', error));

}

const getOrders = () => {
    console.log("in getOrders")
    console.log(`${uri}/order/GetAllOrdersToSupplier/${localStorage.getItem('supplierId')}`);
    let loader = document.getElementById('loader');
    loader.style.display = 'block';
    loader.innerHTML = 'Loading data...';
    fetch(`${uri}/order/GetAllOrdersToSupplier/${localStorage.getItem('supplierId')}`)
        .then(response => response.json())
        .then(data => {
            loader.style.display = 'none';
            _displayItems(data)
        })
        .catch(error => console.error('Unable to get items.', error));
}

const _displayItems = (data) => {

    const tBody = document.getElementById('ordersDetails');
    tBody.innerHTML = "";
    console.log(data);
    let welcome = document.getElementById('welcome');
    welcome.innerHTML += `שלום ${localStorage.getItem('supplierName')}`;
    if (data == null || data.status == 404 || data.length == 0) {
        const table = document.getElementById('table');
        table.innerHTML = `<h2>אין הזמנות לספק</h2>`;
        return;
    }
    data.forEach(item => {

        let tr = tBody.insertRow();
        let td1 = tr.insertCell(0);
        td1.innerText = item.grocerId;
        let td2 = tr.insertCell(1);
        let list = document.createElement('ul');
        item.products.forEach(product => {
            let li = document.createElement('li');
            li.innerText = `${product.productName} ${product.minQuantity}`;
            list.appendChild(li);
        })
        td2.appendChild(list);
        let td3 = tr.insertCell(2);
        let nav = document.createElement('nav');
        nav.innerText = item.status;
        td3.appendChild(nav);
        td3.id = `status_${item._id}`;
        td3.appendChild(document.createElement('br'));
        let changeStatusButton = document.createElement('button');
        changeStatusButton.innerText = 'שינוי סטטוס';
        changeStatusButton.setAttribute('onclick', `changeStatus(status_${item._id})`);
        changeStatusButton.value = item.status;
        if (item.status != "ממתינה") {
            changeStatusButton.setAttribute('disabled', true);
        }
        td3.appendChild(changeStatusButton)

    });
}
window.onload = getOrders;

