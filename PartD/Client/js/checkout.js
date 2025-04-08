const uri = 'https://localhost:7085/api';


const sendOrder=()=>{
    const jsonOrder=document.getElementById('jsonOrder');
        
        fetch(`${uri}/goods/UpdateGoodsSupply`, {
            method: 'PUT',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body:JSON.stringify({
                "tomato": 9,
                "bread": 7
            })
        }).then(response => console.log("succeed"))
        .catch(error => console.error('Unable to update item.', error));

}
//example page for testing checkout and automatic order