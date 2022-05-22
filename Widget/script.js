const galeryFrame = document.getElementById("galeryFrame");
const galeryIndicators = document.getElementById("galeryIndicators");
const closeRegistrationFormButton = document.getElementById("closeRegistrationFormButton");
const openRegistrationDialog = document.getElementById("openRegistrationDialog");
const registrationSubmitButton = document.getElementById("registrationSubmitButton");

galeryFrame.addEventListener("click", rotatePics);
closeRegistrationFormButton.addEventListener("click", HideRegistrationForm);
openRegistrationDialog.addEventListener("click", ShowRegistrationForm);
registrationSubmitButton.addEventListener("click", registrationSubmitAction);

PlacingIndicators();

function rotatePics()
{
   for (var i = 0; i < galeryFrame.children.length; i++){
      if (galeryFrame.children[i].className == "galery-frameItem-visible")
      {
         galeryFrame.children[i].className
            = "galery-frameItem-invisible";
         galeryFrame.children[(i + 1) % galeryFrame.children.length].className
            = "galery-frameItem-visible";

         switchIndicators((i + 1) % galeryFrame.children.length);
         break;
      }
   };
};

function PlacingIndicators(){
   for (var i = 0; i < galeryFrame.children.length; i++){
      let span = document.createElement("span");
      span.className = "galery-indicatorsItem";
      if (galeryFrame.children[i].className == "galery-frameItem-visible"){
         span.classList.add("selected");
      }
      galeryIndicators.appendChild(span);
   }
};

function switchIndicators(index){
   for (var i = 0; i < galeryIndicators.children.length; i++){
      galeryIndicators.children[i].className = "galery-indicatorsItem";
   }
   galeryIndicators.children[index].classList.add("selected");
};

function ShowRegistrationForm(){
   let div = document.querySelector(".registrationForm");
   div.style.display = "flex";
};

function HideRegistrationForm(){
   let div = document.querySelector(".registrationForm");
   div.style.display = "none";
};

function registrationSubmitAction(event){
   event.preventDefault();
   HideRegistrationForm();
   let fullName = document.getElementById("fullNameInput").innerHTML;
   let email = document.getElementById("emailInput").innerHTML;

   document.getElementById("FullNameTableCell").innerHTML = fullName;
   document.getElementById("EmailTableCell").innerHTML = fullName;
}