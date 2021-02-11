﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookMyFood.ServiceChat {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="LeaderServer", Namespace="http://schemas.datacontract.org/2004/07/BookMyFood.ServerFunction")]
    [System.SerializableAttribute()]
    public partial class LeaderServer : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ServerDNSNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ServerIPField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VisibleNameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServerDNSName {
            get {
                return this.ServerDNSNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ServerDNSNameField, value) != true)) {
                    this.ServerDNSNameField = value;
                    this.RaisePropertyChanged("ServerDNSName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServerIP {
            get {
                return this.ServerIPField;
            }
            set {
                if ((object.ReferenceEquals(this.ServerIPField, value) != true)) {
                    this.ServerIPField = value;
                    this.RaisePropertyChanged("ServerIP");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string VisibleName {
            get {
                return this.VisibleNameField;
            }
            set {
                if ((object.ReferenceEquals(this.VisibleNameField, value) != true)) {
                    this.VisibleNameField = value;
                    this.RaisePropertyChanged("VisibleName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ServerUser", Namespace="http://schemas.datacontract.org/2004/07/BookMyFoodWCF")]
    [System.SerializableAttribute()]
    public partial class ServerUser : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ID {
            get {
                return this.IDField;
            }
            set {
                if ((this.IDField.Equals(value) != true)) {
                    this.IDField = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceChat.IServiceChat", CallbackContract=typeof(BookMyFood.ServiceChat.IServiceChatCallback))]
    public interface IServiceChat {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceChat/Connect", ReplyAction="http://tempuri.org/IServiceChat/ConnectResponse")]
        int Connect(string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceChat/Connect", ReplyAction="http://tempuri.org/IServiceChat/ConnectResponse")]
        System.Threading.Tasks.Task<int> ConnectAsync(string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceChat/Disconnect", ReplyAction="http://tempuri.org/IServiceChat/DisconnectResponse")]
        void Disconnect(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceChat/Disconnect", ReplyAction="http://tempuri.org/IServiceChat/DisconnectResponse")]
        System.Threading.Tasks.Task DisconnectAsync(int id);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServiceChat/SendMsg")]
        void SendMsg(string msg, int id);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServiceChat/SendMsg")]
        System.Threading.Tasks.Task SendMsgAsync(string msg, int id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChatCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServiceChat/MsgCallBack")]
        void MsgCallBack(string msg);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChatChannel : BookMyFood.ServiceChat.IServiceChat, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceChatClient : System.ServiceModel.DuplexClientBase<BookMyFood.ServiceChat.IServiceChat>, BookMyFood.ServiceChat.IServiceChat {
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceChatClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public int Connect(string name) {
            return base.Channel.Connect(name);
        }
        
        public System.Threading.Tasks.Task<int> ConnectAsync(string name) {
            return base.Channel.ConnectAsync(name);
        }
        
        public void Disconnect(int id) {
            base.Channel.Disconnect(id);
        }
        
        public System.Threading.Tasks.Task DisconnectAsync(int id) {
            return base.Channel.DisconnectAsync(id);
        }
        
        public void SendMsg(string msg, int id) {
            base.Channel.SendMsg(msg, id);
        }
        
        public System.Threading.Tasks.Task SendMsgAsync(string msg, int id) {
            return base.Channel.SendMsgAsync(msg, id);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceChat.IWebService")]
    public interface IWebService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWebService/Check", ReplyAction="http://tempuri.org/IWebService/CheckResponse")]
        BookMyFood.ServiceChat.LeaderServer Check();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWebService/Check", ReplyAction="http://tempuri.org/IWebService/CheckResponse")]
        System.Threading.Tasks.Task<BookMyFood.ServiceChat.LeaderServer> CheckAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWebService/UsersList", ReplyAction="http://tempuri.org/IWebService/UsersListResponse")]
        BookMyFood.ServiceChat.ServerUser[] UsersList();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWebService/UsersList", ReplyAction="http://tempuri.org/IWebService/UsersListResponse")]
        System.Threading.Tasks.Task<BookMyFood.ServiceChat.ServerUser[]> UsersListAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWebServiceChannel : BookMyFood.ServiceChat.IWebService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WebServiceClient : System.ServiceModel.ClientBase<BookMyFood.ServiceChat.IWebService>, BookMyFood.ServiceChat.IWebService {
        
        public WebServiceClient() {
        }
        
        public WebServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WebServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public BookMyFood.ServiceChat.LeaderServer Check() {
            return base.Channel.Check();
        }
        
        public System.Threading.Tasks.Task<BookMyFood.ServiceChat.LeaderServer> CheckAsync() {
            return base.Channel.CheckAsync();
        }
        
        public BookMyFood.ServiceChat.ServerUser[] UsersList() {
            return base.Channel.UsersList();
        }
        
        public System.Threading.Tasks.Task<BookMyFood.ServiceChat.ServerUser[]> UsersListAsync() {
            return base.Channel.UsersListAsync();
        }
    }
}
