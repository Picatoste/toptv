
/**
 * App 
 */
 var nelements = 0
var App = new (Backbone.Router.extend({

  routes: {
    "solucion/:id": "show", //matches "solucion/1" y "solucion/1/"
    "soluciones(/)": "index", //matches "soluciones" y "soluciones/"
	"aboutme": "aboutme", //matches "aboutme"
    "*any" : "redirect" //matches anything else *wildcard ;)
     },

  initialize: function(){
		(function(d, s, id) {
		  var js, fjs = d.getElementsByTagName(s)[0];
		  if (d.getElementById(id)) return;
		  js = d.createElement(s); js.id = id;
		  js.src = "//connect.facebook.net/es_LA/all.js#xfbml=1&appId=595636333827563";
		  fjs.parentNode.insertBefore(js, fjs);
		}(document, 'script', 'facebook-jssdk'));

  },

  start: function(n){

    //creamos lista de entradas sin filtrar
    //mediante bootstrapping :)
	nelements = n
    this.entradasList = new EntradaList().first(n);
    
    //lista filtrada que se usará para generar las vistas
    this.activeList = null;

    this.filter='';

    Backbone.history.start();
  },

  redirect: function()
  {
    //redirigimos a ruta inicial
    this.navigate("soluciones/",true);
  },
	
  aboutme: function(){
	  $("#app-main").fadeOut(2000, function(x){ $("#main").attr("class", "wrapper clearfix");});
	  $("#app-aboutme").fadeIn(4000);
			
  },

  index: function(){
		this.show(15);
		$("#app-aboutme").fadeOut(2000, function(x){ $("#main").attr("class", "main wrapper clearfix");});
		$("#app-main").fadeIn(4000);
  },

  show: function(id)
  {
	$("html, body").animate({ scrollTop: 0 }, "slow");
    //creamos objetos
    var entrada = new EntradaItem({id: id});
    var entradaView = new EntradaFichaView({model: entrada});

    //limpiamos ui
    $('#ui').empty();

    //añadimos vista ficha
    $('#app').html(entradaView.el);

    //pedimos datos al server
    entrada.fetch({id: id});
    
    //initialize
    if(!this.activeList)
      this.activeList = new EntradaList(this.entradasList.models);
	 
	 this.entradasList = this.activeList;
    var entradasView = new EntradaListaView({collection: this.activeList});

    //generamos vista
    $('#ui').html(_.template($('#searchTemplate').html(),
      //escape to prevent XSS attack
      { filter: _.escape(this.filter), sortOrder: this.activeList.sortOrder, sortField: this.activeList.sortField }
    ));
    
    $('#app-side').html(entradasView.el);

    //pedimos datos al server
    this.activeList.fetch();
	this.activeList = this.activeList.first(nelements);
    //render
    //no haria falta si cargamos del server ya que reaccionamos a los eventos :)
    entradasView.render();
	
	//$('#buttonLikeIt').append('<fb:like href="' + encodeURIComponent(document.URL) + '" send="true" width="450" show_faces="true" font="tahoma"></fb:like>');
	$(".fecha").each(function( index ) {
		var dateString = $.trim($(this).text());
		var year = dateString.substring(0,4);
		var month = dateString.substring(4,6);
		var day = dateString.substring(6,8);
		$(this).text(day + '/' + month + '/' + year);
	});
	

		
	 var modelSocialShare = new SocialSharing({readUrl: entrada.get("url_public"), name: entrada.get("titulo"), message: entrada.get("header")});
     this.socialSharingView = new SocialSharingView({model: modelSocialShare});
     this.socialSharingView.render();
	 
	 if (typeof(FB) != 'undefined' && FB != null ) {
		FB.XFBML.parse(); 
	 }


  },

  //ordenar por attr field en order order
  sortBy: function(field, order)
  {
    this.activeList.sortField = field;
    this.activeList.sortOrder = order;
    this.activeList.sort();
  },

  //filtrar entradas
  filterBy: function(filter)
  {
    this.filter = filter;

    //"remember" sort options
    var field = this.activeList.sortField;
    var order = this.activeList.sortOrder;

    this.activeList = this.entradasList.search(this.filter, nelements);

    //sort results
    this.activeList.sortField = field;
    this.activeList.sortOrder = order;
    this.activeList.sort();

    //creamos nueva vista con datos filtrados
    var entradasView = new EntradaListaView({collection: this.activeList });

    //regeneramos vista
    entradasView.render();
    $('#app-side').html(entradasView.el);

  }

}))();