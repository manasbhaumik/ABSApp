//Custom javascript/jquery

$( function(){
    setContentHeight();

    $( window ).resize(function() {
        setContentHeight();
    });

    $('.btn-user .dropdown-toggle').on('click', function (event) {
        $(this).parent().toggleClass('show');
    });

    $('.btn-user .user-close').on('click', function (event) {
        $(this).parents('.btn-user').removeClass('show');
    });

    $('body').find('.header .main-menu .navbar-toggler').on('click', function(){

        if($('body').hasClass('is-sidemenu')){
            $('body').removeClass('is-sidemenu');
        }
        else{
            $('body').addClass('is-sidemenu');            
        }
    });

    // $('body').on('click',function (e) {
    //     if ( 
    //         $(this).hasClass('dropdown-toggle-user') === false
    //         && $(this).hasClass('btn-user-img') === false
    //         && $('.btn-user').hasClass('.show') === true
    //     ) {            
    //         $('.btn-user').removeClass('show');
    //     }

        
    //     if(
    //         !$(e.target).hasClass('navbar-toggler') 
    //         &&
    //         !$(e.target).hasClass('navbar-toggler-icon')
    //         &&
    //         $(this).parents('.navbar-nav-side').length < 1
    //         &&
    //         $('body').hasClass('is-sidemenu') === true
    //     ) {            
    //         $('body').removeClass('is-sidemenu');

    //         $('body').find('.navbar-toggler').removeClass('show');
    //         $('body').find('.navbar-toggler').attr('aria-expanded', false);

    //         $('body').find('.navbar-nav-side').removeClass('show');
    //         $('body').find('.navbar-nav-side').attr('aria-expanded', false);
    //     }

    // });

    $('body').on('click', '.content .content-header .content-search .input-group .input-group-addon',function(){
        $(this).parent().toggleClass('active');
    });

    $('body').on('click', '.accordion-appointment .a-time-slot .form-group .btn-radio',function(){
        console.log('radio');
        $('body').find('.modal-timeslot .modal-seleted-timeslot').text($(this).siblings('label').text());
        $('body').find('#timeslotModalConfirm').modal('show');
    });

});

function setContentHeight(){
    var setHeight = 'auto';
    
    if( $('body').find('.header') && $('body').find('.content-header') && $('body').find('.content-body') ){

        if($(window).width() > 574) {
            setHeight = $(window).height() - $('body').find('.header').height() - $('body').find('.content-header').height() - 2;
        }        

        $('body').find('.content-body').css('min-height',setHeight);

        if($('body').find('.accordion-message') ){
            $('body').find('.accordion-message .collapse-content').css('min-height',setHeight);
        }

        if($('body').find('.appointment-right') ){
            $('body').find('.appointment-right').css('min-height',setHeight);
        }

    }
}