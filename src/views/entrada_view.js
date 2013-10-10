/**
 * Ver index.html para las templates
 */

/**
 * EntradaView 
 * Vista item entrada para listado
 * encapsulado en <article></article>
 */
EntradaView = Backbone.View.extend({

	//renderizar vista cuando el modelo cambie
	initialize: function()
	{
		//this.model.on("change", this.render, this);
		this.listenTo(this.model, "change", this.render);
	},

	render: function()
	{
		this.$el.html(_.template($('#itemTemplate').html(),this.model.attributes));
	}
});

/**
 * EntradaFichaView 
 * Vista item entrada en ficha
 * encapuslado en <div></div>
 */
EntradaFichaView = Backbone.View.extend({
	//renderizar vista cuando el modelo cambie
	initialize: function()
	{
		// this.model.on("change", this.render, this);
		this.listenTo(this.model, "change", this.render);
	},

	render: function()
	{
		this.$el.html(_.template($('#fichaTemplate').html(),this.model.attributes));
		document.title = 'SOLUCIONES.NET por DBR' + this.model.get("titulo");
		$("meta[property='og\\:title']").attr("content", 'SOLUCIONES .NET por DBR - ' + this.model.get("titulo"));
		$("meta[property='og\\:description']").attr("content", this.model.get("header"));
		$("meta[property='og\\:image']").attr("content", document.URL.split("index.html")[0] + $('#imageSolucion').attr('src').trim());
		
		SyntaxHighlighter.highlight();	
	}
});

/**
 * EntradaListaView
 * Listado de items entrada
 * encpasulado en <ul></ul>
 */
EntradaListaView = Backbone.View.extend({

	initialize: function()
	{
		//renderizar vista cuando la coll se resetee (datos nuevos)
		//o haya un evento sort (ordenar)
		this.listenTo(this.collection, "reset", this.render);
		this.listenTo(this.collection, "sort", this.render);
		// this.collection.on("reset", this.render, this);
		// this.collection.on("sort", this.render, this);
		//this.collection.on("add", this.addOne, this);

	},

	addAll: function()
	{
		this.collection.forEach(this.addOne, this);
	},

	render: function()
	{
		this.$el.empty();
		this.addAll();
	},

	addOne: function(item)
	{
		var itemView = new EntradaView({model: item});
		itemView.render();
		this.$el.append(itemView.el);
	}

});
