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
    [SerializeField] GameObject addressObject;//00.アドレスの入力画面

    [SerializeField] GameObject initialSelectObject;//01.保護者側/高齢者側の選択画面
    [SerializeField] GameObject certificationObject;//02.保護者側の認証コード入力選択画面

    [SerializeField] GameObject enterNameObject;//12.名前、決定/戻るの選択画面
    [SerializeField] GameObject confirmationSelectObject;//13.確認事項、決定/キャンセルの選択画面

    string mail = null;//入力したメールアドレス(Gmail)
    string pass = null;//入力したパスワード(password)

    string code = null;//入力した認証コード
    string name = null;//入力した名前

    int initialSelect = 0;//01.
    //int enterNameSelect = 0;//12.
    //int confirmationSelect = 0;//13.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        addressObject.SetActive(true);//00.

        initialSelectObject.SetActive(false);//01.
        certificationObject.SetActive(false);//02

        enterNameObject.SetActive(false);//12.
        confirmationSelectObject.SetActive(false);//13.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnValueMailChanged()//メールアドレス入力判定
    {
        string inputMailString = _InputMailField.text; // 文字列を取得
        _MailText.text = inputMailString;              // テキストに反映する場合

        mail = inputMailString;
        Debug.Log("Mail;  " + mail);
    }
    public void OnValuePassChanged()//パスワード入力判定
    {
        string inputPassString = _InputPassField.text; // 文字列を取得
        _PassText.text = inputPassString;              // テキストに反映する場合
        pass = inputPassString;
        Debug.Log("Pass;  " + pass);
    }

    public void OnValueCodeChanged()//認証コード入力判定
    {
        string inputCodeString = _InputCodeField.text; // 文字列を取得
        _CodeText.text = inputCodeString;              // テキストに反映する場合

        code = inputCodeString;
        Debug.Log("Code;  " + code);
    }
    public void OnValueNameChanged()//名前入力判定
    {
        string inputNameString = _InputNameField.text; // 文字列を取得
        _NameText.text = inputNameString;              // テキストに反映する場合
        name = inputNameString;
        Debug.Log("Name;  " + name);
    }

    public void DecisionAddressButton()//アドレス決定ボタン
    {
        if (mail == null | mail == "")//記入なしの場合
        {
            Debug.Log("mail を入力してください。");

            return;
        }
        if (pass == null | pass == "")//記入なしの場合
        {
            Debug.Log("pass を入力してください。");

            return;
        }

        addressObject.SetActive(false);//00
        initialSelectObject.SetActive(true);//01

        Debug.Log(initialSelect);
    }

    public void GuardianButton(int initialSelect)//保護者側ボタン
    {
        initialSelectObject.SetActive(false);//01
        initialSelect = 1;
        certificationObject.SetActive(true);//02

        Debug.Log(initialSelect);
    }

    public void SeniorButton()//高齢者側ボタン
    {
        initialSelectObject.SetActive(false);//01
        initialSelect = 2;
        enterNameObject.SetActive(true);//12

        Debug.Log(initialSelect);
    }

    public void DecisionCertificationButton()//認証コード決定ボタン
    {
        if (code == null | code == "")//記入なしの場合
        {
            Debug.Log("code を入力してください。");

            return;
        }

        certificationObject.SetActive(false);//02
        confirmationSelectObject.SetActive(true);//13

        Debug.Log(initialSelect);
    }
    public void ReturnCertificationButton()//認証コードから戻るボタン
    {
        certificationObject.SetActive(false);//02
        initialSelectObject.SetActive(true);//01

        Debug.Log(initialSelect);
    }

    public void DecisionNameButton()//名前決定ボタン
    {
        if (name == null | name == "")//記入なしの場合
        {
            Debug.Log("name を入力してください。");

            return;
        }

        enterNameObject.SetActive(false);//12
        confirmationSelectObject.SetActive(true);//13
    }
    public void ReturnNameButton(int initialSelect)//名前から戻るボタン
    {
        enterNameObject.SetActive(false);//12
        initialSelectObject.SetActive(true);//01
    }

    public void DecisionButton()//決定ボタン
    {
        confirmationSelectObject.SetActive(false);//13
    }

    public void CancelButton(int initialSelect)//やり直すボタン
    {
        confirmationSelectObject.SetActive(false);//13
        initialSelectObject.SetActive(true);//01
    }

    /*
        //audioSource.PlayOneShot(gameStartSE);
        //シーン遷移
        //GameManager.level = level;//ステージの難易度設定
        //SceneManager.LoadScene("GameScene");//シーン切り替え
        Initiate.Fade("GameScene01", Color.black, 1.0f);//移動先のシーン#色指定#フェードにかかる時間
     */
}
