const uri = 'https://localhost:7085/api';
const statusList = ["ממתינה" , "בתהליך","הושלמה"];

const changeStatus = (order) => {
    let status=order.querySelector('nav');
    let changeStatusButton=order.querySelector('button');
    status.innerText = statusList[(statusList.indexOf(status.innerText) + 1) % statusList.length];
    if (status.innerText != "ממתינה") {
        changeStatusButton.setAttribute('disabled', true);
    }
}

const getOrders = () => {
    console.log("in getOrders")
    console.log(`${uri}/order/GetAllOrdersToSupplier/${localStorage.getItem('supplierId')}`);
    fetch(`${uri}/order/GetAllOrdersToSupplier/${localStorage.getItem('supplierId')}`)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

const _displayItems = (data) => {
    const tBody = document.getElementById('ordersDetails');
    tBody.innerHTML = "";
    console.log(data);
    let welcome=document.getElementById('welcome');
    welcome.innerHTML+=`שלום ${localStorage.getItem('supplierName')}`;
    data.forEach(item => {

        let tr = tBody.insertRow();
        let td1 = tr.insertCell(0);
        td1.innerText = item.grocerId;

        let td2 = tr.insertCell(1);
        // td2.innerText = item.
        // ;
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
        // td3.appendChild(document.createElement('p').innerHTML= item.status);
        td3.appendChild(nav);
        td3.id=`status_${item._id}`;
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

{/* <button id="changeStatus" type="button" 
    onclick="changeStatus()" value="ממתינה">שינוי סטטוס</button> */}