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
		
		//this.listenTo(this.collection, "reset", this.render);
		//this.listenTo(this.collection, "sort", this.render);
		
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
		this.collection = this.collection.first(5);
		this.addAll();
	},

	addOne: function(item)
	{
		var itemView = new EntradaView({model: item});
		itemView.render();
		this.$el.append(itemView.el);
	}

});

SocialSharingView = Backbone.View.extend({
  el: "#share-item",
 
  events: {
    'click #URLButton': 'onURL'
  },
 
  template:
	'<div class="left margin_left_10"><a href="https://www.facebook.com/dialog/send?app_id=595636333827563&name=SolucionesDBR - <%= encodeURIComponent(name) %>&description=<%= encodeURIComponent(message) %>&link=<%= encodeURIComponent(readUrl) %>&redirect_uri=https://apps.facebook.com/solucionesDBR/" target="_blank"><img class="right selectable" src="img/facebook_orange.png" title="Compartir facebook" alt="Compartir FB"></img></a></div>' +
    '<div class="left margin_left_10"><a href="https://twitter.com/share?url=<%= encodeURIComponent(readUrl) %>&text=<%= encodeURIComponent(name) %>" target="_blank"><img class="right selectable" src="img/twitter_orange.png" title="Compartir twitter" alt="Compartir TW"></img></a></div>'+
    '<div class="left margin_left_10"><a href="mailto:friend@somewhere.com?subject=SolucionesDBR - <%= name %>&body=by <%= author %>, <%= message %> <%= readUrl %>" target="_blank"><img class="right selectable" src="img/email_orange.png" title="Compartir email" alt="Compartir Email"></img></a></div>' +
    '<div class="left margin_left_10"><a href="https://plus.google.com/share?url=<%= encodeURIComponent(readUrl) %>" target="_blank"><img class="right selectable" src="img/plus_orange.png" title="Compartir Google+" alt="Compartir GPlus"></img></a></div>',
	 

 
  render: function () {
    var dataContext = this.model.toJSON();
    var compiledTemplate = _.template(this.template);
    var html = compiledTemplate(dataContext);
 
    this.$el.html(html);
  },
 
  onURL: function () {
    alert('Give this URL to anyone! ' + this.model.get('readUrl'));
  }
});

