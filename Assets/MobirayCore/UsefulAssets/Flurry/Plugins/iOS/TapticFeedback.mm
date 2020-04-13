//
//  TapticFeedback.mm
//  Unity-iPhone
//
//  Created by Mark Rassamni on 3/29/17.
//
//

void Taptic(int type){
    switch (type) {
        case 0:{
            UINotificationFeedbackGenerator *generator =  [[UINotificationFeedbackGenerator alloc] init];
            [generator notificationOccurred:UINotificationFeedbackTypeWarning];
            break;
        }
        case 1:{
            UINotificationFeedbackGenerator *generator =  [[UINotificationFeedbackGenerator alloc] init];
            [generator notificationOccurred:UINotificationFeedbackTypeError];
            break;
        }
        case 2:{
            UINotificationFeedbackGenerator *generator =  [[UINotificationFeedbackGenerator alloc] init];
            [generator notificationOccurred:UINotificationFeedbackTypeSuccess];
            break;
        }
        case 3:{
            UIImpactFeedbackGenerator *generator =  [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
            [generator impactOccurred];
            [generator prepare];
            break;
        }
        case 4:{
            UIImpactFeedbackGenerator *generator =  [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
            [generator impactOccurred];
            [generator prepare];
            break;
        }
        case 5:{
            UIImpactFeedbackGenerator *generator =  [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
            [generator impactOccurred];
            [generator prepare];
            break;
        }
        case 6:{
            UISelectionFeedbackGenerator *generator = [[UISelectionFeedbackGenerator alloc] init];
            [generator selectionChanged];
            break;
        }
        default:
            //Do nothing, should never reach here
            break;
    }
}

extern "C"{
    void _PlayTaptic(char* type){
        if(strcmp(type, "warning") == 0){
            Taptic(0);
        } else if (strcmp(type, "error") == 0){
            Taptic(1);
        } else if (strcmp(type, "success") == 0){
            Taptic(2);
        } else if (strcmp(type, "light") == 0){
            Taptic(3);
        } else if (strcmp(type, "medium") == 0){
            Taptic(4);
        } else if (strcmp(type, "heavy") == 0){
            Taptic(5);
        } else if (strcmp(type, "selectionChange") == 0){
            Taptic(6);
        } else {
            printf("TapticFeedback: Attempted to pass an invalid taptic type to _PlayTaptic");
        }
    }
}

