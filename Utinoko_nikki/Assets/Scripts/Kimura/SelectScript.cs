using System.Net;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI; // LegacyのUIコンポーネントを使うために必要

public class SelectScript : MonoBehaviour
{
    //SE
    [SerializeField] AudioClip SE;//SE
    AudioSource audioSource;

    [SerializeField] InputField _InputMailField;//
    [SerializeField] Text _MailText;//
    [SerializeField] InputField _InputPassField;//
    [SerializeField] Text _PassText;//

    [SerializeField] InputField _InputCodeField;//
    [SerializeField] Text _CodeText;//
    [SerializeField] InputField _InputNameField;//
    [SerializeField] Text _NameText;//

    //public GameObject initialSelectObject;//
    [SerializeField] GameObject initialSelectObject;//00.保護者側/高齢者側の選択画面
    [SerializeField] GameObject addressObject;//01.アドレスの入力画面

    [SerializeField] GameObject certificationObject;//11.保護者側の認証コード入力選択画面
    [SerializeField] GameObject enterNameObject;//12.名前、決定/戻るの選択画面

    [SerializeField] GameObject confirmationSelectObject;//40.確認事項、決定/キャンセルの選択画面

    [SerializeField] GameObject completionObject;//50.入力完了画面

    [SerializeField] Text _ERROR_Text01;//「メールアドレス」と「パスワード」を入力すべきと表示するためのエラーテキスト
    [SerializeField] Text _ERROR_Text11;//「認証コード」を入力すべきと表示するためのエラーテキスト
    [SerializeField] Text _ERROR_Text12;//「名前」を入力すべきと表示するためのエラーテキスト
    [SerializeField] Text _Content_Confirmation_Text;//確認事項時の、入力内容を表示するためのテキスト

    string mail = null;//入力したメールアドレス(Gmail)
    string pass = null;//入力したパスワード(password)

    string code = null;//入力した認証コード
    string name = null;//入力した名前

    int initialSelect = 0;//0=未設定/1=保護者側/2=高齢者側、判定

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialSelectObject.SetActive(true);//00.保護者側/高齢者側の選択画面

        addressObject.SetActive(false);//01.アドレスの入力画面
        _ERROR_Text01.enabled = false;//エラーテキスト非表示

        certificationObject.SetActive(false);//11.保護者側の認証コード入力選択画面
        _ERROR_Text11.enabled = false;//エラーテキスト非表示

        enterNameObject.SetActive(false);//12.名前、決定/戻るの選択画面
        _ERROR_Text12.enabled = false;//エラーテキスト非表示

        confirmationSelectObject.SetActive(false);//40.確認事項、決定/キャンセルの選択画面

        completionObject.SetActive(false);//50.入力完了画面
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnValueMailChanged()//メールアドレス入力判定
    {
        string inputMailString = _InputMailField.text; // 文字列を取得
        //_MailText.text = inputMailString;              // テキストに反映する場合

        mail = inputMailString;
        Debug.Log("Mail;  " + mail);
    }
    public void OnValuePassChanged()//パスワード入力判定
    {
        string inputPassString = _InputPassField.text; // 文字列を取得
        //_PassText.text = inputPassString;              // テキストに反映する場合
        pass = inputPassString;
        Debug.Log("Pass;  " + pass);
    }

    public void OnValueCodeChanged()//認証コード入力判定
    {
        string inputCodeString = _InputCodeField.text; // 文字列を取得
        //_CodeText.text = inputCodeString;              // テキストに反映する場合

        code = inputCodeString;
        Debug.Log("Code;  " + code);
    }
    public void OnValueNameChanged()//名前入力判定
    {
        string inputNameString = _InputNameField.text; // 文字列を取得
        //_NameText.text = inputNameString;              // テキストに反映する場合
        name = inputNameString;
        Debug.Log("Name;  " + name);
    }

    public void FamilyButton()//保護者側ボタン
    {
        initialSelectObject.SetActive(false);//00.保護者側/高齢者側の選択画面
        initialSelect = 1;
        addressObject.SetActive(true);//01.アドレスの入力画面
        _ERROR_Text11.enabled = false;//エラーテキスト非表示

        Debug.Log(initialSelect);
    }

    public void ElderButton()//高齢者側ボタン
    {
        initialSelectObject.SetActive(false);//00.保護者側/高齢者側の選択画面
        initialSelect = 2;
        addressObject.SetActive(true);//01.アドレスの入力画面
        _ERROR_Text12.enabled = false;//エラーテキスト非表示

        Debug.Log(initialSelect);
    }

    public void DecisionAddressButton()//アドレス決定ボタン
    {
        if (mail == null | mail == "")//記入なしの場合
        {
            _ERROR_Text01.text = "メールアドレスを入力してください。";
            _ERROR_Text01.enabled = true;//エラーテキスト表示
            //Debug.Log("mail を入力してください。");

            return;
        }
        if (pass == null | pass == "")//記入なしの場合
        {
            _ERROR_Text01.text = "パスワードを入力してください。";
            _ERROR_Text01.enabled = true;//エラーテキスト表示
            //Debug.Log("pass を入力してください。");

            return;
        }

        if (initialSelect == 1)//保護者側の場合
        {
            addressObject.SetActive(false);//01.アドレスの入力画面
            certificationObject.SetActive(true);//11.保護者側の認証コード入力選択画面
        }
        else if (initialSelect == 2)//高齢者側の場合
        {
            addressObject.SetActive(false);//01.アドレスの入力画面
            enterNameObject.SetActive(true);//12.名前、決定/戻るの選択画面
        }
    }

    public void DecisionCertificationButton()//認証コード決定ボタン
    {
        if (code == null | code == "")//記入なしの場合
        {
            _ERROR_Text11.text = "認証コードを入力してください。";
            _ERROR_Text11.enabled = true;//エラーテキスト表示
            //Debug.Log("code を入力してください。");

            return;
        }

        certificationObject.SetActive(false);//11.保護者側の認証コード入力選択画面
        confirmationSelectObject.SetActive(true);//40.確認事項、決定/キャンセルの選択画面

        _Content_Confirmation_Text.text = code;//次の場面で表示される確認画面のテキストを、入力した認証コードに変更
    }
    public void ReturnCertificationButton()//認証コードから戻るボタン
    {
        certificationObject.SetActive(false);//11.保護者側の認証コード入力選択画面
        addressObject.SetActive(true);//01.アドレスの入力画面
        _ERROR_Text01.enabled = false;//エラーテキスト非表示

        Debug.Log(initialSelect);
    }

    public void DecisionNameButton()//名前決定ボタン
    {
        if (name == null | name == "")//記入なしの場合
        {
            _ERROR_Text12.text = "名前を入力してください。";
            _ERROR_Text12.enabled = true;//エラーテキスト表示
            //Debug.Log("name を入力してください。");

            return;
        }

        enterNameObject.SetActive(false);//12.名前、決定/戻るの選択画面
        confirmationSelectObject.SetActive(true);//40.確認事項、決定/キャンセルの選択画面

        _Content_Confirmation_Text.text = name;//次の場面で表示される確認画面のテキストを、入力した名前に変更
    }
    public void ReturnNameButton()//名前から戻るボタン
    {
        enterNameObject.SetActive(false);//12.名前、決定/戻るの選択画面
        addressObject.SetActive(true);//01.アドレスの入力画面
        _ERROR_Text01.enabled = false;//エラーテキスト非表示
    }

    public void DecisionButton()//決定ボタン
    {
        confirmationSelectObject.SetActive(false);//40.確認事項、決定/キャンセルの選択画面
        completionObject.SetActive(true);//50.入力完了画面
    }

    public void CancelButton()//やり直すボタン
    {
        if (initialSelect == 1)//保護者側の場合
        {
            confirmationSelectObject.SetActive(false);//40.確認事項、決定/キャンセルの選択画面
            certificationObject.SetActive(true);//11.保護者側の認証コード入力選択画面
            _ERROR_Text11.enabled = false;//エラーテキスト非表示
        }
        else if (initialSelect == 2)//高齢者側の場合
        {
            confirmationSelectObject.SetActive(false);//40.確認事項、決定/キャンセルの選択画面
            enterNameObject.SetActive(true);//12.名前、決定/戻るの選択画面
            _ERROR_Text12.enabled = false;//エラーテキスト非表示
        }
    }
}
