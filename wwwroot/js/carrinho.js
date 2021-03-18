class Carrinho {

    clickPlusQtd(btn) {

        let dataObj = this.getData(btn);
        dataObj.Quantidade++;
        this.postUpdateQuantidade(dataObj);
    }

    clickMinusQtd(btn) {

        let dataObj = this.getData(btn);
        dataObj.Quantidade--;
        this.postUpdateQuantidade(dataObj);
    }

    getData(element) {
        var linhaItem = $(element).parents('[item-id]')
        var itemId = $(linhaItem).attr('item-id');
        var qtd = $(linhaItem).find('input').val();

        return {
            Id: itemId,
            Quantidade: qtd
        };
    }

    postUpdateQuantidade(dataObj) {

        let tokenInput = $('[name=__RequestVerificationToken]').val();
        let headers = {};
        headers['RequestVerificationToken'] = tokenInput;

        $.ajax({
            url: '/pedido/updatequantidade',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataObj),
            headers: headers
        }).done(function (response) {

            //Atualiza informações da tela
            let itemPedido = response.itemPedido;
            let linhaDoItem = $('[item-id=' + itemPedido.id + ']');
            let carrinhoViewModel = response.carrinhoViewModel;

            linhaDoItem.find('input').val(itemPedido.quantidade);
            linhaDoItem.find('[subtotal]').html((itemPedido.subtotal).duasCasasDecimais());
            $('[numero-itens]').html('Total:' + carrinhoViewModel.itens.length + 'itens');
            $('[total]').html((carrinhoViewModel.total).duasCasasDecimais());

            if (itemPedido.quantidade == 0) {
                linhaDoItem.remove();
            }
        });
    }

    blurInputQuantidade(input) {
        let dataObj = this.getData(input);
        this.postUpdateQuantidade(dataObj);
    }

}

var carrinho = new Carrinho();

Number.prototype.duasCasasDecimais = function () {
    return this.toFixed(2).replace('.', ',');
}
