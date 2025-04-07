const uri = 'https://localhost:7085/api/supplier';


const enterSystem = async () => {
    console.log("in enterSystem");

    const enterForm = document.getElementById('enterForm');
    enterForm.style.display = 'none';
    localStorage.setItem('isLoggedIn', true);
    const supplierPhone = document.getElementById('supplier-phone').value;
    const supplierName = document.getElementById('supplier-name').value;
    console.log(`${uri}/${supplierPhone}`);

    await fetch(`${uri}/GetSupplierByPhone/${supplierPhone}`)
        .then(response => response.json())
        .then(data => {
            console.log(data);
            if (data == null || data.status == 404) {
                alert("מספר טלפון לא קיים במערכת")
                enterForm.style.display = 'block';
                return;
            }
            if (data.companyName != supplierName) {
                alert("שם חברה לא תואם מספר טלפון");
                enterForm.style.display = 'block';
                return;
            }
            //all is good need to show details
            localStorage.setItem('supplierId', data._id);
            localStorage.setItem('supplierName', data.companyName);
            localStorage.setItem('supplierPhone', data.phone);
            localStorage.setItem('supplierContactPerson', data.contactPerson);
            window.location.href = '/supplierDetails.html';
        })
        .catch(error => {
            alert("נתונים שגויים הכנס שוב");
            enterForm.style.display = 'block';
            console.error('Unable to get supplier.', error)
            return;
        });


}
const showNewSupplierDetails = () => {
    const newSupplierInfoButton = document.getElementById('newSupplierInfoButton')
    newSupplierInfoButton.style.display = 'none';
    const newSupplierInfo = document.getElementById('newSupplierInfo');
    console.log("in showNewSupplierDetails")
    newSupplierInfo.style.display = 'block';
    let newSupplierCompanyName = document.createElement('input');
    newSupplierCompanyName.id = 'new-supplier-company-name';
    newSupplierCompanyName.type = 'text';
    newSupplierCompanyName.placeholder = 'שם חברה';
    newSupplierInfo.appendChild(newSupplierCompanyName);
    newSupplierInfo.append(document.createElement('br'));
    newSupplierInfo.append(document.createElement('br'));
    let newSupplierPhoneNumber = document.createElement('input');
    newSupplierPhoneNumber.id = 'new-supplier-phone-number';
    newSupplierPhoneNumber.type = 'text';
    newSupplierPhoneNumber.placeholder = 'מספר טלפון';
    newSupplierInfo.appendChild(newSupplierPhoneNumber);
    newSupplierInfo.append(document.createElement('br'));
    newSupplierInfo.append(document.createElement('br'));
    const newSupplierContactPerson = document.createElement('input');
    newSupplierContactPerson.id = 'new-supplier-contact-person';
    newSupplierContactPerson.type = 'text';
    newSupplierContactPerson.placeholder = 'שם נציג';
    newSupplierInfo.appendChild(newSupplierContactPerson);
    newSupplierInfo.append(document.createElement('br'));
    newSupplierInfo.append(document.createElement('br'));
    const newSupplierProductButton = document.getElementById('new-supplier-product-button');
    newSupplierProductButton.style.display = 'block';
    // newSupplierProductButton.innerText = 'הוספת מוצר';
    // newSupplierProductButton.onclick = addProductField();
    // newSupplierInfo.appendChild(newSupplierProductButton);
    newSupplierInfo.append(document.createElement('br'));
    const submitButtom = document.createElement('button');
    submitButtom.type = 'submit';
    submitButtom.innerHTML = 'שלח';
    newSupplierInfo.appendChild(submitButtom);

}

const addProductField = (e) => {
    // e.preventDefault();
    const newProductDiv = document.createElement('div')
    newProductDiv.className = 'new-product-div'
    console.log("in addProductField")
    const newSupplierInfo = document.getElementById('newSupplierInfo');
    const newProductNameInput = document.createElement('input');
    newProductNameInput.placeholder = 'שם מוצר';
    newProductNameInput.id = 'new-product-name';
    newProductNameInput.type = 'text';
    newProductDiv.appendChild(newProductNameInput);
    const newProductPrice = document.createElement('input');
    newProductPrice.placeholder = 'מחיר';
    newProductPrice.min = 0;
    newProductPrice.id = 'new-product-price';
    newProductPrice.type = 'number';
    newProductDiv.appendChild(newProductPrice);
    const newProductQuantity = document.createElement('input');
    newProductQuantity.placeholder = 'כמות מינימלית להזמנה';
    newProductQuantity.id = 'new-product-quantity';
    newProductQuantity.type = 'number';
    newProductDiv.appendChild(newProductQuantity);
    newSupplierInfo.appendChild(newProductDiv);

}


function addSupplier() {
    const newSupplierCompanyName = document.getElementById('new-supplier-company-name').value.trim();
    const newSupplierConcatPerson = document.getElementById('new-supplier-contact-person').value.trim();
    const newSupplierPhoneNumber = document.getElementById('new-supplier-phone-number').value;
    const newSpplierproducts = document.getElementsByClassName('new-product-div');

    const item = {
        _id:generateUUID(),
        companyName: newSupplierCompanyName,
        Phone: newSupplierPhoneNumber,
        contactPerson: newSupplierConcatPerson,
        Products: []
    };
    for (let i = 0; i < newSpplierproducts.length; i++) {
        const product = {
            productName: newSpplierproducts.item(i).querySelector('#new-product-name').value,
            pricePerItem: newSpplierproducts.item(i).querySelector('#new-product-price').value,
            minQuantity: newSpplierproducts.item(i).querySelector('#new-product-quantity').value

        }
        item.Products.push(product);    
        console.log(item.Products);
    }

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .catch(error => console.error('Unable to add item.', error));
        window.location.href = '/supplierDetails.html';

}

function generateUUID() { // Public Domain/MIT
    var d = new Date().getTime();//Timestamp
    var d2 = ((typeof performance !== 'undefined') && performance.now && (performance.now()*1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
    return 'xxxxxxxxxx4xxxyxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16;//random number between 0 and 16
        if(d > 0){//Use timestamp until depleted
            r = (d + r)%16 | 0;
            d = Math.floor(d/16);
        } else {//Use microseconds since page-load if supported
            r = (d2 + r)%16 | 0;
            d2 = Math.floor(d2/16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}