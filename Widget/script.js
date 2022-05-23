AddEventListeners();



function AddEventListeners(){
   let widgetColumns = document.querySelectorAll(".widgetColumnt");
   widgetColumns.forEach(element => {
      let addButton = element.lastElementChild;
      addButton.addEventListener("click", ShowChoosingWidgetDialog);

      element.firstElementChild.addEventListener('dragover', WidgetColunmDragoveredHandler);
      element.firstElementChild.addEventListener('dragleave', WidgetColunmDragleavedHandler);
   });
}

function WidgetColunmDragoveredHandler(event)
{
   event.preventDefault();
   if(dragged.parentElement === event.target.parentElement)
      return;
   dragovered = event.target;
}

function WidgetColunmDragleavedHandler(event){
   dragovered = undefined;
}

let dragged = undefined;
let dragovered = undefined;


function ShowChoosingWidgetDialog(event){
   let dialog = document.querySelector(".choosingWidgetDialog");
   dialog.style.display = "flex";
}

class Widget{
   constructor(bgcolor, tcolor, labelText){
      this.bgcolor = bgcolor;
      this.tcolor = tcolor;
      this.Shell = document.createElement("div");
      this.Shell.classList.add("widget");
      this.Shell.style.color = tcolor;
      this.Shell.style.background = bgcolor;
      this.Label = document.createElement("h2");

      this.Shell.addEventListener('dragstart', WidgetDragStartHandler);
      this.Shell.addEventListener('dragend', WidgetDragEndHandler);

      this.Label.innerHTML = labelText;
   }
   Insert(parent){
      parent.appendChild(this.ToHTMLElement());
   }

   ToHTMLElement(){
      this.Shell.appendChild(this.Label);

      this.Shell.draggable = true;

      return this.Shell;
   }
}

function WidgetDragStartHandler(event){
   dragged = event.target;
}
function WidgetDragEndHandler(event){
   if(dragged != undefined && dragovered != undefined)
   {
      dragged.parentElement.removeChild(dragged);
      dragovered.appendChild(dragged);

      console.log(dragovered);

      return;
   }

   dragged = undefined;
   dragovered = undefined;
   return;
}

const firstColumn = document.querySelector(".widgetColumnt").firstElementChild;
let w = new Widget('gray', 'white', 'Widget');
w.Insert(firstColumn);
let w2 = new Widget('blue', 'white', 'another');
w2.Insert(firstColumn);
let w3 = new Widget('gray', 'white', 'Widget');
w3.Insert(firstColumn);