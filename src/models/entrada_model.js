Backbone.sync = function (method, model, options) {
     console.log (method, model, options);
 };
 

/**
 * EntradaItem
 * Modelo de datos de entradas
 */
var EntradaItem = Backbone.Model.extend({

  defaults: { id:0, url_public:'', titulo: '', autor:'', fecha:'', header: '', imagen: '', section:'', footer:'' },
	
  fetch: function(options) {
  			options = options ? _.clone(options) : {};
  			var model = this;
  			var success = options.success;
  			var json 
  			$.ajax(
			{
				type: "GET",
				url: "soluciones.xml",
				dataType: "xml",
				async: false,
				success: function(xml)
				{
					$.each($(xml).find("solucion"), function() {
						if ($(this).find("id").text().trim() == options.id){
							json = JSON.parse(xml2json(this,""));
							
						}
					});
				},
				error: function(xml)
				{
					alert("Error reading solucion");
				}
			});
			this.set("id", json.solucion.id);
			this.set("titulo", json.solucion.titulo);
			this.set("autor", json.solucion.autor);
			this.set("fecha", json.solucion.fecha);
			this.set("header", json.solucion.header);
			this.set("imagen", json.solucion.imagen);
			this.set("section", json.solucion.section['#cdata']);
			this.set("footer", json.solucion.footer['#cdata']);
			this.set("url_public", json.solucion.url_public);
			
		   return (this.sync || Backbone.sync).call(this, 'read', this, options);
	}
	    
});

/**
 * EntradaList 
 * Collection de EntradaItem
 */
var EntradaList = Backbone.Collection.extend({
	model: EntradaItem,
	sortOrder: "desc",
	sortField: "fecha",

	sortByField: function (fieldName) {
            this.sort_key = fieldName;
            this.sort();
	},

	comparator: function (entrada1, entrada2)
	{
		if(this.sortOrder === "asc")
		{
			return entrada1.get(this.sortField) > entrada2.get(this.sortField);

		}else //this.sortOder === "desc"
		{
			return entrada1.get(this.sortField) < entrada2.get(this.sortField);
		}
	},

	search: function(letters, nelements)
	{
		if(letters === "") return this;

		//filtramos por regexp, i flag para ignore case (no distinguir lowercase/uppercase)
		var pattern = new RegExp(letters,'i');

		//iteramos la coll
		var filteredList = this.filter(function(data)
		{
			return (pattern.test( data.get('autor') + "  " + data.get('titulo') ));
			//podríamos filtrar por editorial también
			//return ( pattern.test(data.get('autor')) || pattern.test(data.get('titulo')) );// || pattern.test(data.get('fecha')) );
		});

		//create new coll con los elementos filtrados
		var coll = new EntradaList(filteredList).first(nelements);

		return coll;

	},

	
  fetch: function(options) {
  			options = options ? _.clone(options) : {};
  			var collection = this;
  			var success = options.success;
  			var collectionJson
  			var xmlResult 
  			$.ajax(
			{
				type: "GET",
				url: "soluciones.xml",
				dataType: "xml",
				async: false,
				success: function(xml)
				{
					xmlResult = xml
				},
				error: function(xml)
				{
					alert("Error reading solucion");
				}
			});
		
			$.each($(xmlResult).find("solucion"), function() {
				var json;	
				json = JSON.parse(xml2json(this,""));
				
				var EntradaItemAdd = new EntradaItem();
				EntradaItemAdd.set("id", json.solucion.id);
				EntradaItemAdd.set("titulo", json.solucion.titulo);
			    EntradaItemAdd.set("autor", json.solucion.autor);
				EntradaItemAdd.set("fecha", json.solucion.fecha);
				EntradaItemAdd.set("header", json.solucion.header);
				EntradaItemAdd.set("imagen", json.solucion.imagen);
				EntradaItemAdd.set("section", json.solucion.section['#data']);
				EntradaItemAdd.set("footer", json.solucion.footer['#data']);
				EntradaItemAdd.set("url_public", json.solucion.url_public);
				collection.push(EntradaItemAdd, '');			
			});
			
			collection.sort_key = "fecha";
            collection.sort();
			
		   return (this.sync || Backbone.sync).call(this, 'readCollection', this, options);
	}


});

var SocialSharing = Backbone.Model.extend({
  defaults: {
    author: 'David Benito',
    name: 'Soluciones DBR',
    message: 'Soluciones DBR',
    readUrl: 'http://www.solucionesdbr.es/'
  }
});
