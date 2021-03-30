using ProcessWatcher.Core;
using ProcessWatcher.ViewModels;
using ProcessWatcher.Views;
using ReactiveUI;
using Splat;

namespace ProcessWatcher
{
	public interface IMainScreen : IScreen
	{

	}

	// public class LoginBootstrapper : ReactiveObject, ILoginScreen
	// {
	// 	public RoutingState Router { get; }
	//
	// 	public LoginBootstrapper(IReadonlyDependencyResolver resolver = null, IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
	// 	{
	// 		Router = testRouter ?? new RoutingState();
	// 		dependencyResolver ??= Locator.CurrentMutable;
	// 		RegisterParts(dependencyResolver);
	// 	}
	//
	// 	/// <summary>The register parts.</summary>
	// 	/// <param name="dependencyResolver">The dependency resolver.</param>
	// 	private void RegisterParts(IMutableDependencyResolver dependencyResolver)
	// 	{
	//
	// 	}
	//
	// }
	public class AppBootstrapper : ReactiveObject, IMainScreen
	{
		public RoutingState Router { get; }
		/// <summary>Initializes a new instance of the <see cref="AppBootstrapper"/> class.</summary>
		/// <param name="resolver">The dependency injection resolver which will resolve the programs dependencies.</param>
		/// <param name="dependencyResolver">The dependency injection resolver which will allow registration of new dependencies.</param>
		/// <param name="testRouter">The router.</param>
		public AppBootstrapper(IReadonlyDependencyResolver resolver = null, IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
		{
			Router = testRouter ?? new RoutingState();
			resolver ??= Locator.Current;
			dependencyResolver ??= Locator.CurrentMutable;

			RegisterParts(dependencyResolver);

			// Navigate to the opening page of the application
			// Router.Navigate.Execute(resolver.GetService<IMainViewModel>()).Subscribe();
			// Navigate to the opening page of the application
			// resolver.GetService<ILoginScreen>().Router.Navigate.Execute(resolver.GetService<ILoginViewModel>()).Subscribe();

		}


		/// <summary>The register parts.</summary>
		/// <param name="dependencyResolver">The dependency resolver.</param>
		private void RegisterParts(IMutableDependencyResolver dependencyResolver)
		{

			var locator = Locator.Current;
			// Make sure Splat and ReactiveUI are already configured in the locator
			// so that our override runs last
			Locator.CurrentMutable.InitializeSplat();
			Locator.CurrentMutable.InitializeReactiveUI();
			// var logger = new CustomLogger(LogLevel.Debug);
			// dependencyResolver.RegisterConstant<ILogger>(logger);
			dependencyResolver.RegisterConstant<IMainScreen>(this);
			dependencyResolver.RegisterConstant<ILogsViewModelFactory>(new LogsViewModelFactory());
			dependencyResolver.Register<IMainViewModel>(() => new MainViewModel(locator.GetService<IMainScreen>(), RxApp.MainThreadScheduler));
			dependencyResolver.Register<IViewFor<IMainViewModel>>(() => new MainView());
			dependencyResolver.Register<IViewFor<IProcessViewModel>>(() => new ProcessView());
			dependencyResolver.Register<IViewFor<ILogsViewModel>>(() => new LogsView());

			// dependencyResolver.RegisterConstant<IViewLocator>(new ConventionalViewLocator());
			// dependencyResolver.RegisterConstant<IContextMenuViewModelFactory>(new ContextMenuViewModelFactory());
			// dependencyResolver.RegisterConstant<IEditViewModelFactory>(new EditViewModelFactory());
			// dependencyResolver.RegisterConstant<IGenericObservableViewModelFactory>(new GenericObservableViewModelFactory());
			// dependencyResolver.RegisterConstant<IRecordingSessionCreationViewModelFactory>(new RecordingSessionCreationViewModelFactory());
			// dependencyResolver.RegisterConstant<IRecordingSessionEditViewModelFactory>(new RecordingSessionEditViewModelFactory());
			// dependencyResolver.RegisterConstant<IRoomViewModelFactory>(new RoomViewModelFactory());
			// dependencyResolver.RegisterConstant<IMicrophoneViewModelFactory>(new MicrophoneViewModelFactory());
			// dependencyResolver.RegisterConstant<IOperatorViewModelFactory>(new OperatorViewModelFactory());
			// dependencyResolver.RegisterConstant<ICameraViewModelFactory>(new CameraViewModelFactory());
			// dependencyResolver.RegisterConstant<IRecordingSessionViewModelFactory>(new RecordingSessionViewModelFactory());


			// dependencyResolver.Register<IRoomViewModelFactory>(() => new ControllersViewModel(RxApp.MainThreadScheduler, global, locator.GetService<IMainScreen>()));

			// dependencyResolver.RegisterConstant<ILoginScreen>(new LoginBootstrapper());
			// var global = new GlobalClass();
			// var global = new GlobalMock();
			// dependencyResolver.RegisterConstant<IGlobal>(global);

			// dependencyResolver.RegisterConstant<ILoginViewModelFactory>(new DefaultLoginViewModelFactory());
			// dependencyResolver.RegisterConstant<IRepositoryFactory>(new DefaultRepositoryFactory());
			// dependencyResolver.RegisterConstant<IWindowLayoutViewModel>(new WindowLayoutViewModel());
			// dependencyResolver.Register<ILayoutViewModel>(() => new LayoutViewModel());

			// var locator = Locator.Current;
			// dependencyResolver.Register<ICameraModel>(() => this.ReferenceObservableCollectionsFromGlobal(new Camera(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectCameraNavigatorModel)locator.GetService<ICameraModel>());
			// dependencyResolver.Register<ICamera>(() => new CameraMessage());
			// dependencyResolver.Register<IControllerModel>(() => this.ReferenceObservableCollectionsFromGlobal(new Controller(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectControllerNavigatorModel)locator.GetService<IControllerModel>());
			// dependencyResolver.Register<IController>(() => new ControllerMessage());
			// dependencyResolver.Register<IDecreeModel>(() => this.ReferenceObservableCollectionsFromGlobal(new Decree(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectDecreeNavigatorModel)locator.GetService<IDecreeModel>());
			// dependencyResolver.Register<IDecree>(() => new DecreeMessage());

			// dependencyResolver.Register<IHDVFModel>(() => this.ReferenceObservableCollectionsFromGlobal(new HDVF(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectHDVFNavigatorModel)locator.GetService<IHDVFModel>());
			// dependencyResolver.Register<IHDVF>(() => new HDVFMessage());

			// dependencyResolver.Register<IHOTPTokenModel>(() => this.ReferenceObservableCollectionsFromGlobal(new HOTPToken(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectHOTPTokenNavigatorModel)locator.GetService<IHOTPTokenModel>());
			// dependencyResolver.Register<IHOTPToken>(() => new HOTPTokenMessage());

			// dependencyResolver.Register<ILinkedServerModel>(() => this.ReferenceObservableCollectionsFromGlobal(new LinkedServer(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectLinkedServerModel)locator.GetService<ILinkedServerModel>());
			// dependencyResolver.Register<ILinkedServer>(() => new LinkedServerMessage());
			// dependencyResolver.Register<IMicrophoneModel>(() => this.ReferenceObservableCollectionsFromGlobal(new Microphone(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectMicrophoneNavigatorModel)locator.GetService<IMicrophoneModel>());
			// dependencyResolver.Register<IMicrophone>(() => new MicrophoneMessage());
			// dependencyResolver.Register<IOperatorModel>(() => this.ReferenceObservableCollectionsFromGlobal(new Operator(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectOperatorModel)locator.GetService<IOperatorModel>());
			// dependencyResolver.Register<IOperator>(() => new OperatorMessage());
			// dependencyResolver.Register<IPTZModel>(() => this.ReferenceObservableCollectionsFromGlobal(new PTZ(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectPTZNavigatorModel)locator.GetService<IPTZModel>());
			// dependencyResolver.Register<IPTZ>(() => new PTZMessage());
			// dependencyResolver.Register<IPTZSettingModel>(() => this.ReferenceObservableCollectionsFromGlobal(new PTZSetting(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectPTZSettingModel)locator.GetService<IPTZSettingModel>());
			// dependencyResolver.Register<IPTZSetting>(() => new PTZSettingMessage());
			// dependencyResolver.Register<IRecordingSessionModel>(() => this.ReferenceObservableCollectionsFromGlobal(new RecordingSession(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectRecordingSessionNavigatorModel)locator.GetService<IRecordingSessionModel>());
			// dependencyResolver.Register<IRecordingSession>(() => new RecordingSessionMessage());
			// dependencyResolver.Register<IRoomModel>(() => this.ReferenceObservableCollectionsFromGlobal(new Room(), global));
			// dependencyResolver.Register(() => (ICloneableReactiveObjectRoomModel)locator.GetService<IRoomModel>());
			// dependencyResolver.Register<IRoom>(() => new RoomMessage());

			// dependencyResolver.Register<IMainViewModel>(() => new MainViewModel(locator.GetService<IMainScreen>(), global, RxApp.MainThreadScheduler));
			// dependencyResolver.Register<IViewFor<IMainViewModel>>(() => new MainView());
			// dependencyResolver.Register<IViewFor<IDBRequestViewModel>>(() => new DBRequestView());
			// dependencyResolver.Register<IViewFor<IRoomBlueprintViewModel>>(() => new RoomBlueprintView());
			// dependencyResolver.Register<IDBRequestViewModel>(() => new DBRequestViewModel(locator.GetService<IMainScreen>(), global));
			// // dependencyResolver.Register<ICreatesCommandBinding>(() => new CustomCommandBinder());
			// dependencyResolver.Register<ILoginViewModel>(() => new LoginViewModel(RxApp.MainThreadScheduler));

			// dependencyResolver.Register<IViewFor<IEditViewModel>>(() => new EditView());
			// dependencyResolver.Register<IViewFor<IGenericObservableViewModel>>(() => new GenericObservableView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectCameraNavigatorModel>>(() => new CameraView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectControllerNavigatorModel>>(() => new ControllerView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectHDVFNavigatorModel>>(() => new HDVFView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectHOTPTokenNavigatorModel>>(() => new HOTPTokenView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectLinkedServerModel>>(() => new LinkedServerView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectMicrophoneNavigatorModel>>(() => new MicrophoneView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectOperatorModel>>(() => new OperatorView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectPTZNavigatorModel>>(() => new PTZView());
			// // dependencyResolver.Register<IViewFor<ICloneableReactiveObjectPTZSettingModel>>(() => new PTZView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectRecordingSessionNavigatorModel>>(() => new RecordingSessionView());
			// dependencyResolver.Register<IViewFor<ICloneableReactiveObjectRoomModel>>(() => new RoomView());
			// dependencyResolver.Register<IViewFor<IRecordingSessionCreationViewModel>>(() => new RecordingSessionCreationView());
			// dependencyResolver.Register<IViewFor<IRecordingSessionEditViewModel>>(() => new RecordingSessionEditView());






			// dependencyResolver.Register<IViewFor<ILoginViewModel>>(() => new LoginView());
			// dependencyResolver.Register<IViewFor<IBranchViewModel>>(() => new BranchesView());
			// dependencyResolver.Register<IViewFor<IRefLogViewModel>>(() => new RefLogView());
			// dependencyResolver.Register<IViewFor<ICommitHistoryViewModel>>(() => new HistoryView());
			// dependencyResolver.Register<IViewFor<IOutputViewModel>>(() => new OutputView());
			// dependencyResolver.Register<IViewFor<IRepositoryDocumentViewModel>>(() => new RepositoryView());
			// dependencyResolver.Register<IViewFor<ITagsViewModel>>(() => new TagView());
		}

		// public T ReferenceObservableCollectionsFromGlobal<T>(T obj, IGlobal global)
		// {
		// 	var observablesCollections = typeof(T).GetProperties().Where(e => e.PropertyType.IsGenericType && e.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ObservableCollection<>))).ToList();
		// 	foreach (var observableCollectionProperty in observablesCollections)
		// 		observableCollectionProperty.SetValue(obj, typeof(IGlobal).GetProperties().FirstOrDefault(e => e.Name == observableCollectionProperty.Name)?.GetValue(global), null );
		// 	return obj;
		// }
		// /// <summary>
		// /// This function is used at IModel first Initialization, cloned will be null only when adding new item
		// /// </summary>
		// /// <param name="cloned"></param>
		// /// <typeparam name="T"></typeparam>
		// /// <returns></returns>
		// public T FillNewModel<T>(T cloned = default) where T : IModel, new()
		// {
		// 	T newObject = cloned ?? new T();
		// 	if(newObject is INavigator)
		// 		switch (newObject)
		// 		{
		// 			case ICameraModelNavigator camera:
		// 				camera.Rooms = this.Rooms;
		// 				camera.Controllers = this.Controllers;
		// 				break;
		// 			case IControllerModelNavigator controller:
		// 				controller.Rooms = this.Rooms;
		// 				break;
		// 			case IDecreeModelNavigator decree:
		// 				decree.LinkedServers = this.LinkedServers;
		// 				break;
		// 			case IHOTPTokenModelNavigator hotpToken:
		// 				hotpToken.Operators = this.Operators;
		// 				break;
		// 			case IMicrophoneModelNavigator microphone:
		// 				microphone.Controllers = this.Controllers;
		// 				break;
		// 			case IPTZModelNavigator ptz:
		// 				ptz.Cameras = this.Cameras;
		// 				break;
		// 			case IRecordingSessionModelNavigator recordingSession:
		// 				recordingSession.Cameras = this.Cameras;
		// 				recordingSession.Decrees = this.Decrees;
		// 				break;
		// 			// case IRoomModel room:
		// 			// case IOperatorModel @operator:
		// 			// case ILinkedServerModel linkedServer:
		// 			// 	break;
		// 		}
		//
		// 	return newObject;
		// }

	}
}