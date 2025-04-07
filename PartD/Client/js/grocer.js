const uri = 'https://localhost:7085/api';


const enterSystem = () => {
    const enterForm = document.getElementById('enterForm');
    enterForm.style.display = 'none';
    const grocerId = document.getElementById('grocer-id');
    localStorage.setItem('grocerId', grocerId.value);
    const grocerPage=document.getElementById('grocerPage');
    grocerPage.style.display = 'block';

    showOrderForm();
}

const showOrderForm = () => {
    fetch(`${uri}/supplier/GetAllSuppliers`)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

const _displayItems = (data) => {
    const tBody = document.getElementById('supplierDetails');
    tBody.innerHTML = "";
    console.log(data);

    data.forEach(item => {

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        td1.id=`grocer_${item._id}`;
        td1.innerText = item.companyName;
        td1.innerText+=`\n `;
        td1.innerText+=item.phone;

        let td2 = tr.insertCell(1);
        td2.innerText = item.contactPerson;

        let td3 = tr.insertCell(2);
        item.products.forEach(product => {
            let li = document.createElement('li');
            li.innerText = `${product.productName}  `;           
            let input = document.createElement('input');
            input.type = 'number';
            input.value = product.minQuantity;
            input.id=`product_${product._id}`;
            input.min = product.minQuantity;
            li.appendChild(input);
            let isWantToOrderItem= document.createElement('input');
            isWantToOrderItem.type = 'checkbox';
            isWantToOrderItem.id=`wantToOrder_${product._id}`;
            li.appendChild(isWantToOrderItem);
            td3.appendChild(li);
            td3.className='productsList';
        })

    });

}
const makeOrder=()=>{
    const productsList=document.querySelectorAll('.productsList');
    let orderList=[];
    console.log(productsList);
    productsList.forEach(item=>{
        console.log(item);
        
    })
}