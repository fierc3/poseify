import useEstimations from "../../helpers/estimations";

const Dashboard = () => {

  const {data: estimations} = useEstimations();
  return (
    <div className="App">
        Dashboard
        {estimations?.map((x,i) => <div key={i}>{x.displayName}</div>)}
    </div>
  );
}

export default Dashboard;
