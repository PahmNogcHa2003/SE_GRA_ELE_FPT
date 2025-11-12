import 'package:hola_bike_app/data/sources/remote/api_kyc.dart';
import 'package:hola_bike_app/domain/models/info_kyc.dart';
import 'package:image_picker/image_picker.dart';

class KycUsecase {
  final KycApi _api = KycApi();

  Future<KycInfo> execute({
    required String token,
    required String jsonDataString,
    required XFile frontImage,
    required XFile backImage,
  }) {
    return _api.submitKyc(
      token: token,
      jsonDataString: jsonDataString,
      frontImage: frontImage,
      backImage: backImage,
    );
  }
}
