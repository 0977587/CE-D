Container = require('./Container');
const FormApp = () => {
  const triggerText = "Open form";
  const onSubmit = (event) => {
    event.preventDefault(event);
    console.log(event.target.name.value);
    console.log(event.target.email.value);
  };
  return (
    <div className="App">
      <Container triggerText={triggerText} onSubmit={onSubmit} />
    </div>
  );
};

export default FormApp;
