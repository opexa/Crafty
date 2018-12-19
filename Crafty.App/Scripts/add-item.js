$(document).ready(function(){
  resetSess();
  // Add event listener to the nine image upload buttons.
  // Take their data and trigger the appropriate file input
  $(".img-upload-btn").on("click", function(ev){
    var fieldId = this.getAttribute("data-input-id");
    var field = $("#pictures-container").find("[data-field-id='" + fieldId + "']");
    field.trigger("click");
  });

  // Url reader, for dynamic image preview
  function readURL(input, inputId) {
    if (input.files && input.files[0]) {
      var reader = new FileReader();

      reader.onload = function (e) {
        $("[data-show-id='" + inputId + "']").find("img").attr('src', e.target.result);
      }

      reader.readAsDataURL(input.files[0]);
    }
  }

  // On change of a file input, show the image
  // hide the upload button and show the delete button
  $(".item-img-upload").change(function(){
      var inputId = this.getAttribute("data-field-id");
      readURL(this, inputId);
      hideCurrentInput(inputId);
      showCurrentManager(inputId);
  });

  // On click of the remove preview button,
  // remove the picture, empty the input and show the
  // upload img button
  $(".img-remove-preview").click(function() {
    var boxId = this.getAttribute("data-show-id");
    showCurrentInput(boxId);
    hideCurrentManager(boxId);
  });

  function changeInput(inputId) {
    console.log(inputId);
  }

  // Simple function for hiding upload image button
  function hideCurrentInput(inputId) {
    var input = $("[data-input-id='" + inputId + "']");
    input.addClass("hidden-upload-btn");
  }

  // Simple function for showing delete preview button
  function showCurrentManager(inputId) {
    var manager = $("[data-show-id='" + inputId + "']");
    manager.removeClass("preview-hidden");
  }

  // Simple function for showing upload image button
  function showCurrentInput(inputId) {
    var input = $("[data-input-id='" + inputId + "']");
    $("[data-field-id='" + inputId +"']").val("");
    input.removeClass("hidden-upload-btn");
  }

  // Simple function for hiding delete preview button
  function hideCurrentManager(boxId) {
    var manager = $("[data-show-id='" + boxId + "']");
    manager.addClass("preview-hidden");
  }

  // Click and drag scroll function.
  // Used for the category decision box
  $.fn.attachDragger = function(){
    var attachment = false, lastPosition, position, difference;
    $( $(this).selector ).on("mousedown mouseup mousemove",function(e){
        if( e.type == "mousedown" ) attachment = true, lastPosition = [e.clientX, e.clientY];
        if( e.type == "mouseup" ) attachment = false;
        if( e.type == "mousemove" && attachment == true ){
            position = [e.clientX, e.clientY];
            difference = [ (position[0]-lastPosition[0]), (position[1]-lastPosition[1]) ];
            // $(this).scrollLeft( $(this).scrollLeft() - difference[0] );
            $(this).scrollTop( $(this).scrollTop() - difference[1] );
            lastPosition = [e.clientX, e.clientY];
        }
    });
    $(window).on("mouseup", function(){
        attachment = false;
    });
  }

  // Attaching click and drag scroll function on the
  // categories decision box
  $("#categories-wrapper").attachDragger();

  // Show the category decision box on click
  $("#add-item-category").click(function(){
    showManager();
  });

  // On click outside of the item category manager, hide the manager
  $(".category-manager").on('click', function(ev){
    ev.stopPropagation();
    var target = $(ev.target);
    if(!$(".sections-container").hasClass("hidden-container") && !target.parents("div.sections-container").length) {
      return hideManager();
    }

    if(!$(".categories-container").hasClass("hidden-container") && !target.parents("div.categories-container").length) {
      return hideManager();
    }
  });

  // Subcategory decision box popup.If the previously selected category
  // is the same as the current, don't send AJAX and show the old decision box
  $(".section-option").click(function(ev) {
    var sectionId = this.getAttribute("data-section-id");

    $.ajax({
      type: "GET",
      url: "/items/categories/" + sectionId,
      success: function(data) {
        showCategories(data, sectionId);
        selectPotSection(sectionId);
        markUpCategory(sessionStorage.currCtg);
      },
      error: function(err) {
        console.log(err.responseText);
      }
    });
  });

  // Go back function for the subcategory decision box.
  $(".back-button").click(function(ev) {
    ev.stopPropagation();
    $(".categories-container").addClass("hidden-container");
    $(".sections-container").removeClass("hidden-container");
  });

  $(".escape-button").click(function() {
    hideManager();
  });

  // On click on subcategory, update the item category and hide the category manager.
  $("div.categories-container").on('click', 'div.category-option', function (ev) {
    var categoryId = this.getAttribute("data-id");
    $("input#CategoryId").val(categoryId);
    selectCategory(categoryId);
    hideManager();
    $("#add-item-category").html($("div.category-option[data-id='" + categoryId + "']").html());
    $('span[data-valmsg-for="CategoryId"]').html("");
  });

  // Mark up chosen category and unmark prevous
  function selectSection(clickedId) {
    $(".marked-section-option").removeClass("marked-section-option");
    $("div.section-option[data-section-id='" + clickedId + "']").addClass("marked-section-option");
  }

  function selectPotSection(clickedId){
    sessionStorage.potSec = clickedId;
  }

  function markUpCategory(id) {
    $("div.category-option[data-id='" + id + "']").addClass("marked-category-option");
  }

  // Mark up chosen subcategory and unmark prevous
  function selectCategory(clickedId) {
    selectSection(sessionStorage.potSec);
    $(".marked-category-option").removeClass("marked-category-option");
    sessionStorage.currCtg = clickedId;
    $("div.category-option[data-id='" + clickedId + "']").addClass("marked-category-option");
  }

  // Hide the category manager
  function hideManager() {
    $(".category-manager").addClass("hidden-manager");
    $(".categories-container").addClass("hidden-container");
  }

  // Show the category manager
  function showManager(){
    $(".category-manager").removeClass("hidden-manager");
    $(".sections-container").removeClass("hidden-container");
  }

  // Show the subcategories decision box with the returned data from AJAX
  function showCategories(data, categoryId){
    $("#categories-wrapper").html(data);

    $(".categories-container").removeClass("hidden-container");
    $(".section-name").html($("div.section-option[data-section-id='" + categoryId +"']").html());
    $(".sections-container").addClass("hidden-container");
  }

  function resetSess(){
    sessionStorage.removeItem("currCtg");
    sessionStorage.removeItem("potSec");
  }
});
