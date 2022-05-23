const choosingWidgetDialogFrame = document.querySelector(".choosingWidgetDialog-frame");
choosingWidgetDialogFrameInit();

function choosingWidgetDialogFrameInit(){
   let span = document.createElement("span");
   span.classList.add("choosingWidgetDialog-option");
   span.innerHTML = "Погода";
   span.addEventListener("click", AddWeatherOption);
   choosingWidgetDialogFrame.appendChild(span);
   span = document.createElement("span");
   span.classList.add("choosingWidgetDialog-option");
   span.innerHTML = "Курс валют";
   span.addEventListener("click", AddCurrencyOption);
   choosingWidgetDialogFrame.appendChild(span);
}

function AddWeatherOption(){
   let wid = new WeatherWidget('lightblue', 'white');
   wid.Insert(activeColumn);
   activeColumn = undefined;
   HideChoosingWidgetDialog();
}

function AddCurrencyOption(){
   let wid = new CurrencyWidget('lightGray', 'black');
   wid.Insert(activeColumn);
   activeColumn = undefined;
   HideChoosingWidgetDialog();
}

AddEventListeners();

const RuCitiesToEng = new Map()
   .set("Лондон", "London")
   .set("Москва", "Moscow")
   .set("Вашингтон", "Washington")
   .set("Сидней", "Sydney")
   .set("Париж", "Paris")
   .set("Берлин", "Berlin")
   .set("Санкт-Петербург", "St. Petersburg")
   .set("Ижевск", "Izhevsk")
   .set("Прага", "Prague");

const CurrenciesList = ["RUB", "EUR", "USD", "CAD", "JPY"];


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
   if(event.target.classList.contains("widgetColumnWrapper"))
      dragovered = event.target;
}

function WidgetColunmDragleavedHandler(event){
   dragovered = undefined;
}

let dragged = undefined;
let dragovered = undefined;


let activeColumn;

function HideChoosingWidgetDialog(){
   let dialog = document.querySelector(".choosingWidgetDialog");
   dialog.style.display = "none";
}

function ShowChoosingWidgetDialog(event){
   let dialog = document.querySelector(".choosingWidgetDialog");
   activeColumn = event.target.parentElement.firstElementChild;
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
      this.Header = document.createElement("div");
      this.Header.classList.add("header");
      this.CloseButton = document.createElement("p");
      this.HtmlElement = undefined;

      this.Label = document.createElement("h2");

      this.Shell.addEventListener('dragstart', WidgetDragStartHandler);
      this.Shell.addEventListener('dragend', WidgetDragEndHandler);
      this.CloseButton.addEventListener("click", (e) => {e.target
         .parentElement
         .parentElement
         .parentElement
         .removeChild(this.ToHTMLElement())});

      this.Label.innerHTML = labelText;
      this.CloseButton.innerHTML = "x";
   }
   Insert(parent){
      parent.appendChild(this.ToHTMLElement());
   }

   ToHTMLElement(){
      if(this.HtmlElement == undefined)
      {
         this.Header.appendChild(this.Label);
         this.Header.appendChild(this.CloseButton);

         this.Shell.appendChild(this.Header);
         this.Shell.appendChild(this.GetContent());

   
         this.Shell.draggable = true;

         this.HtmlElement = this.Shell;
      }

      return this.HtmlElement;
   }

   // GetContent(){
   //    console.log('base');
   // }
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

class WeatherWidget extends Widget{
   constructor(bgcolor, tcolor){
      super(bgcolor, tcolor, "Погода");

      this.Content = document.createElement("div");
      this.Content.classList.add("content");

      this.CitySelect = document.createElement("select");
      this.CitySelect.addEventListener("change", this.SetWeatherDataBySelectedOption)

      this.WeatherDataDiv = document.createElement("div");
      this.WeatherDataDiv.classList.add("weatherData");

      this.CityName = document.createElement("p");
      this.CityName.classList.add("weatherData-cityName");

      //

      this.Temperature = document.createElement("p");
      this.Temperature.classList.add("weatherData-temperature");

      this.TemperatureFeel = document.createElement("p");
      this.Temperature.classList.add("weatherData-temperatureFeel");

      this.SpeedOfWind = document.createElement("p");
      this.Temperature.classList.add("weatherData-speedOfWind");

      this.Ultraviolet = document.createElement("p");
      this.Ultraviolet.classList.add("weatherData-uvi");


      this.CitySelectInit();
      this.Build();
      this.SetWeatherDataBySelectedOption();
   }

   SetWeatherDataBySelectedOption = async() =>
   {
      let selected;

      selected = this.CitySelect.options[this.CitySelect.selectedIndex];

      this.CityName.innerHTML = "Погода в г. " + selected.innerHTML;

      let response = await fetch(
         'http://api.weatherapi.com/v1/current.json?key=6a54f93e51704f038aa121957222305&q='
         + RuCitiesToEng.get(selected.innerHTML)
         + '&aqi=no');

      let responseObj = await response.json();

      this.Temperature.innerHTML = "Температура: " + responseObj.current.temp_c + "°C";
      this.TemperatureFeel.innerHTML = "Ощущается как " + responseObj.current.feelslike_c + "°C";
      this.SpeedOfWind.innerHTML = "Скорость ветра: " + responseObj.current.wind_kph + "км/ч";
      this.Ultraviolet.innerHTML = "Ультрафиолет: " + responseObj.current.uv;
   }

   CitySelectInit(){
      for(let e of RuCitiesToEng.keys())
      {
         let option = document.createElement("option");
         option.setAttribute('value', RuCitiesToEng.get(e));
         option.innerHTML = e;
         this.CitySelect.appendChild(option);
      };
   }

   Build(){

      this.WeatherDataDiv.appendChild(this.CityName);
      this.WeatherDataDiv.appendChild(this.Temperature);
      this.WeatherDataDiv.appendChild(this.TemperatureFeel);
      this.WeatherDataDiv.appendChild(this.SpeedOfWind);
      this.WeatherDataDiv.appendChild(this.Ultraviolet);

      this.Content.appendChild(this.CitySelect);
      this.Content.appendChild(this.WeatherDataDiv);
   }

   GetContent(){
      return this.Content;
   }
}

class CurrencyWidget extends Widget{
   constructor(bgcolor, tcolor){
      super(bgcolor, tcolor, "Курс валют");

      this.CurrencyRateValue = 0.5;

      this.Content = document.createElement("div");
      this.Content.classList.add("content");

      this.CurrenciesSelect = document.createElement("div");
      this.CurrenciesSelect.classList.add("currencyCelect");

      this.FromCurrencyDiv = document.createElement("Div");
      this.FromCurrencyLabel = document.createElement("p");
      this.FromCurrencyLabel.innerHTML = "Из: ";
      this.FromCurrencySelect = document.createElement("select");
      this.FromCurrencySelect.addEventListener("change", this.CurrencyRateBySelectedOption);

      this.ToCurrencyDiv = document.createElement("Div");
      this.ToCurrencyLabel = document.createElement("p");
      this.ToCurrencyLabel.innerHTML = "в: ";
      this.ToCurrencySelect = document.createElement("select");
      this.ToCurrencySelect.addEventListener("change", this.CurrencyRateBySelectedOption);

      this.CurrencyRate = document.createElement("p");
      this.CurrencyRate.classList.add("currencyRate");
      this.CurrencyRate.innerHTML = "LALALA";

      this.CurrencyTransfer = document.createElement("div");
      this.CurrencyTransfer.classList.add("CurrencyTransfer");

      this.FirstCurrencyTransferItem = document.createElement("p");
      this.FirstCurrencyTransferItem.classList.add("CurrencyTransfer-Item");

      this.SecondCurrencyTransferItem = document.createElement("p");
      this.SecondCurrencyTransferItem.classList.add("CurrencyTransfer-Item");

      this.FirstCurrencyTransferItemLabel = document.createElement("p");
      this.FirstCurrencyTransferItemLabel.innerHTML = 'XXX: '
      this.FirstCurrencyTransferItemInput = document.createElement("input");
      this.FirstCurrencyTransferItemInput.setAttribute('type', 'number');
      this.FirstCurrencyTransferItemInput.value = 0;
      this.FirstCurrencyTransferItemInput.addEventListener('input', this.FirstInputHandler);

      this.SecondCurrencyTransferItemLabel = document.createElement("p");
      this.SecondCurrencyTransferItemLabel.innerHTML = 'YYY: '
      this.SecondCurrencyTransferItemInput = document.createElement("input");
      this.SecondCurrencyTransferItemInput.setAttribute('type', 'number');
      this.SecondCurrencyTransferItemInput.value = 0;
      this.SecondCurrencyTransferItemInput.addEventListener('input', this.SecondInputHandler);

      this.Build();
      this.CurrenciesSelectInit();
      this.CurrencyRateBySelectedOption();
      //this.SetWeatherDataBySelectedOption();
   }
   
   FirstInputHandler = () =>{
      this.SecondCurrencyTransferItemInput.value = 
      this.FirstCurrencyTransferItemInput.value * this.CurrencyRateValue;
   }

   SecondInputHandler = () =>{
      this.FirstCurrencyTransferItemInput.value = 
      this.SecondCurrencyTransferItemInput.value * this.CurrencyRateValue;
   }

   Build(){
      this.FromCurrencyDiv.appendChild(this.FromCurrencyLabel);
      this.FromCurrencyDiv.appendChild(this.FromCurrencySelect);

      this.ToCurrencyDiv.appendChild(this.ToCurrencyLabel);
      this.ToCurrencyDiv.appendChild(this.ToCurrencySelect);

      this.CurrenciesSelect.appendChild(this.FromCurrencyDiv);
      this.CurrenciesSelect.appendChild(this.ToCurrencyDiv);

      this.FirstCurrencyTransferItem.appendChild(this.FirstCurrencyTransferItemLabel);
      this.FirstCurrencyTransferItem.appendChild(this.FirstCurrencyTransferItemInput);
      this.CurrencyTransfer.appendChild(this.FirstCurrencyTransferItem);

      this.SecondCurrencyTransferItem.appendChild(this.SecondCurrencyTransferItemLabel);
      this.SecondCurrencyTransferItem.appendChild(this.SecondCurrencyTransferItemInput);
      this.CurrencyTransfer.appendChild(this.SecondCurrencyTransferItem);

      this.Content.appendChild(this.CurrenciesSelect);
      this.Content.appendChild(this.CurrencyRate);
      this.Content.appendChild(this.CurrencyTransfer);
   }

   CurrencyRateBySelectedOption = async() =>
   {
      let selectedFrom;
      let selectedTo;

      selectedFrom = this.FromCurrencySelect.options[this.FromCurrencySelect.selectedIndex];
      selectedTo = this.ToCurrencySelect.options[this.ToCurrencySelect.selectedIndex];

      this.FirstCurrencyTransferItemLabel.innerHTML = selectedFrom.innerHTML + ":"
      this.SecondCurrencyTransferItemLabel.innerHTML = selectedTo.innerHTML + ":"
      
      let response = await fetch(
         'https://api.currencyapi.com/v3/latest?apikey=I5PfzfKa0bpRXtvGDyf40bvl4ouHjVDnRHMme3af&currencies='
         + selectedTo.innerHTML + '&base_currency='
         + selectedFrom.innerHTML);

      let responseObj = await response.json();

      //console.log(responseObj.data);

      for(var key in responseObj.data){
         this.CurrencyRateValue = responseObj.data[key].value;
         //console.log(responseObj.data[key].value);
         break;
      }
      

      this.CurrencyRate.innerHTML = "Курс " + selectedFrom.innerHTML + " к " + selectedTo.innerHTML + " = " + this.CurrencyRateValue;
      this.FirstInputHandler();
   }

   CurrenciesSelectInit(){
      for(let currency of CurrenciesList)
      {
         let option = document.createElement("option");
         option.setAttribute('value', currency);
         option.innerHTML = currency;         
         this.FromCurrencySelect.appendChild(option);
         option = document.createElement("option");
         option.setAttribute('value', currency);
         option.innerHTML = currency;
         this.ToCurrencySelect.appendChild(option);
      };
   }

   GetContent(){
      return this.Content;
   }
}

const firstColumn = document.querySelector(".widgetColumnt").firstElementChild;
// let w = new WeatherWidget('gray', 'white');
// w.Insert(firstColumn);
// let w2 = new WeatherWidget('blue', 'white');
// w2.Insert(firstColumn);
// let w3 = new WeatherWidget('gray', 'white');
// w3.Insert(firstColumn);
// let c1 = new CurrencyWidget('gray', 'white');
// c1.Insert(firstColumn);