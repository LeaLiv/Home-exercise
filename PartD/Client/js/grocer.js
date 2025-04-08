const uri = 'https://localhost:7085/api';
const statusList = ["ממתינה", "בתהליך", "הושלמה"];

const changeStatus = async (order) => {
    let status = order.querySelector('nav');
    let changeStatusButton = order.querySelector('button');
    status.innerText = statusList[(statusList.indexOf(status.innerText) + 1) % statusList.length];
    if (status.innerText != "בתהליך") {
        changeStatusButton.setAttribute('disabled', true);
    }
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

const enterSystem = () => {
    const enterForm = document.getElementById('enterForm');
    enterForm.style.display = 'none';
    const grocerId = document.getElementById('grocer-id');
    localStorage.setItem('grocerId', grocerId.value);
    const displayOrderForm = document.getElementById('displayOrderForm');
    displayOrderForm.style.display = 'block';
    const displayOrders = document.getElementById('displayOrders');
    displayOrders.style.display = 'block';

}
const displayOrderForm = () => {
    const grocerPage = document.getElementById('grocerPage');
    grocerPage.style.display = 'block';
    showOrderForm();
}
const showOrderForm = () => {
    let loader = `<div class="loader"></div>`;
    const supplierDetails = document.getElementById('supplierDetails');
    fetch(`${uri}/supplier/GetAllSuppliers`)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}
const displayOrders = () => {
    fetch(`${uri}/order/GetAllOrdersToGrocer/${localStorage.getItem('grocerId')}`)
        .then(response => response.json())
        .then(data => _displayOrders(data))
        .catch(error => console.error('Unable to get items.', error));
}
const _displayOrders = (data) => {
    const displayOrdersTable=document.getElementById('displayOrdersTable');
    displayOrdersTable.style.display='block';
    const tBody = document.getElementById('ordersDetails');
    tBody.innerHTML = "";
    console.log(data);
    data.forEach(item => {

        let tr = tBody.insertRow();
        let td1 = tr.insertCell(0);
        td1.innerText = item.supplierId;

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
        td3.id = `status_${item._id}`;
        td3.appendChild(document.createElement('br'));
        let changeStatusButton = document.createElement('button');
        changeStatusButton.innerText = 'שינוי סטטוס';
        changeStatusButton.setAttribute('onclick', `changeStatus(status_${item._id})`);
        changeStatusButton.value = item.status;
        if (item.status != "בתהליך") {
            changeStatusButton.setAttribute('disabled', true);
        }
        td3.appendChild(changeStatusButton)

    });
}
const _displayItems = (data) => {
    const tBody = document.getElementById('supplierDetails');
    tBody.innerHTML = "";

    data.forEach(item => {

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);

        td1.id = `${item._id}`;
        td1.innerText = item.companyName;
        td1.innerText += `\n `;
        td1.innerText += item.phone;

        let td2 = tr.insertCell(1);
        td2.innerText = item.contactPerson;

        let td3 = tr.insertCell(2);
        item.products.forEach(product => {
            let li = document.createElement('li');
            li.innerText = `${product.productName}  `;
            let input = document.createElement('input');
            input.type = 'number';
            input.value = product.minQuantity;
            input.id = `product_${product.productName}`;
            input.min = product.minQuantity;

            li.appendChild(input);
            let isWantToOrderItem = document.createElement('input');
            isWantToOrderItem.type = 'checkbox';
            isWantToOrderItem.id = `isWantToOrder`;
            li.appendChild(isWantToOrderItem);
            td3.appendChild(li);
            let price = document.createElement('span');
            price.innerText = `${product.pricePerItem}₪`;
            li.appendChild(price);
            td3.className = 'productsList';
        })

    });

}

const makeOrder = () => {
    const productsList = document.querySelectorAll('.productsList');
    let orderList = [];
    productsList.forEach(item => {
        //each product list is a supplier
        let productsList = [];
        let products = item.querySelectorAll('li');
        products.forEach(product => {
            let isWantToOrder = product.querySelector('#isWantToOrder');
            if (isWantToOrder.checked) {
                let productName = product.closest('li').innerText;
                let pricePerItem = product.querySelector('span').innerHTML;
                // console.log(document.createElement('span'));
                pricePerItem = pricePerItem.replace('₪', '')
                pricePerItem = Number(pricePerItem, 10);
                let minQuantity = product.querySelector('input').value;
                let supplierId = product.parentElement.parentElement.id;
                let productItem = {
                    productName: productName,
                    pricePerItem: pricePerItem,
                    minQuantity: minQuantity
                }
                productsList.push(productItem);
            }
        })
        console.log(item.parentElement.closest('tr').querySelector('td').id);
        if (productsList.length != 0) {
            let orderItem = {
                _id: generateUUID(),
                supplierId: item.parentElement.closest('tr').querySelector('td').id,
                grocerId: localStorage.getItem('grocerId'),
                products: productsList,
                status: "ממתינה"
            }
            orderList.push(orderItem);

        }

    })

    orderList.forEach(item => {
        console.log(item);
        fetch(`${uri}/order/MakeOrder`, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
            .then(response => response.json())
            .catch(error => console.error('Unable to update item.', error));
    })
    const grocerPage = document.getElementById('grocerPage');
    grocerPage.style.display = 'none';

}

function generateUUID() { // Public Domain/MIT
    var d = new Date().getTime();//Timestamp
    var d2 = ((typeof performance !== 'undefined') && performance.now && (performance.now() * 1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
    return 'xxxxxxxxxx4xxxyxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16;//random number between 0 and 16
        if (d > 0) {//Use timestamp until depleted
            r = (d + r) % 16 | 0;
            d = Math.floor(d / 16);
        } else {//Use microseconds since page-load if supported
            r = (d2 + r) % 16 | 0;
            d2 = Math.floor(d2 / 16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}

