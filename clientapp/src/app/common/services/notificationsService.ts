

export type NotificationType = {
    showMessage: boolean,
    hasMessage: boolean, 
    messageCount: number,
    messages: string[]
}

export default class NotificationsService {
    notificationState: NotificationType = {showMessage: false, hasMessage: false, messageCount: 0, messages: []};

    SetNotificationState = (notificationMsg: string) => {
        if(notificationMsg){
            this.notificationState.messages.push(...notificationMsg);
        }
        this.notificationState.hasMessage = this.notificationState.messages.length > 0;
        this.notificationState.messageCount = this.notificationState.messages.length;
        console.log('Notification State:', JSON.stringify(this.notificationState,null, 2));
        return this.notificationState;
    }

    AddNotificationMessage = (message: string) => {
        if(message){
            this.notificationState.messages.push(message);
            this.notificationState.messageCount = this.notificationState.messages.length;
            this.notificationState.hasMessage = this.notificationState.messageCount > 0;
            // this.notificationState.showMessage = true;
        }
        console.log('Notification State:', JSON.stringify(this.notificationState,null, 2));
        return this.notificationState;
    }

    ToggleShowMessage = () => {
        this.notificationState.showMessage = !this.notificationState.showMessage;
        return this.notificationState;
    }

}